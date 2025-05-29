using Authentication.Application.Interfaces;
using Authentication.Application.Services;
using Authentication.Persistance;
using Authentication.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ENVIRONMENT AND CONFIGURATION
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Release";
var dbPassword = builder.Configuration["DB_PASSWORD"];
var jwtSecret = builder.Configuration["JWT_SECRET"];

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

// DB CONNECTION
var rawConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(dbPassword))
    throw new Exception("Environment variable DB_PASSWORD not set!");

var finalConnectionString = rawConnectionString.Replace("{DB_PASSWORD}", dbPassword);

builder.Services.AddDbContext<AuthenticationDatabaseContext>(options =>
    options.UseNpgsql(finalConnectionString));

// DEPENDENCY INJECTION
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();

builder.Services.AddSingleton<IUserCreatedHandler, UserCreatedHandler>();

builder.Services.AddHostedService<UserCreatedConsumer>();

builder.Services.AddHttpContextAccessor();

// JWT AUTHENTICATION CONFIGURATION

var rawJwtKey = builder.Configuration["Jwt:Key"];
var jwtKey = rawJwtKey?.Replace("{JWT_SECRET}", jwtSecret ?? string.Empty);

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new Exception("JWT secret is not set correctly.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = key
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization(options => {
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("SuperAdmin"));

    options.AddPolicy("Users.Read", policy =>
        policy.RequireClaim("policy", "users:read"));

    options.AddPolicy("Users.Create", policy =>
        policy.RequireClaim("policy", "users:create"));

    options.AddPolicy("Users.Delete", policy =>
        policy.RequireClaim("policy", "users:delete"));

    options.AddPolicy("Users.Update", policy =>
        policy.RequireClaim("policy", "users:update"));
});


var app = builder.Build();

// DATABASE MIGRATION
using (var scope = app.Services.CreateScope()) {
    var authContext = scope.ServiceProvider.GetRequiredService<AuthenticationDatabaseContext>();
    await authContext.Database.MigrateAsync();
}

// MIDDLEWARE PIPELINE
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
