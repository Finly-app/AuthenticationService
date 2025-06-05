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

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: false)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

var dbPassword = builder.Configuration["DB_PASSWORD"];
var jwtSecret = builder.Configuration["JWT_SECRET"];

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
builder.Services.AddHostedService<UserAuthFilterConsumer>();
builder.Services.AddHostedService<UserAuthSingleConsumer>();

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
    options.AddPolicy("RequireSuperAdminRole", policy => policy.RequireRole("SuperAdmin"));
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));

    // Roles
    options.AddPolicy("roles:read", policy => policy.RequireClaim("policy", "roles:read"));
    options.AddPolicy("roles:create", policy => policy.RequireClaim("policy", "roles:create"));
    options.AddPolicy("roles:update", policy => policy.RequireClaim("policy", "roles:update"));
    options.AddPolicy("roles:delete", policy => policy.RequireClaim("policy", "roles:delete"));
    options.AddPolicy("roles:policies:read", policy => policy.RequireClaim("policy", "roles:policies:read"));
    options.AddPolicy("roles:policies:assign", policy => policy.RequireClaim("policy", "roles:policies:assign"));
    options.AddPolicy("roles:policies:remove", policy => policy.RequireClaim("policy", "roles:policies:remove"));
    options.AddPolicy("roles:inheritance:create", policy => policy.RequireClaim("policy", "roles:inheritance:create"));

    // User Roles & Policies
    options.AddPolicy("users:roles:read", policy => policy.RequireClaim("policy", "users:roles:read"));
    options.AddPolicy("users:roles:assign", policy => policy.RequireClaim("policy", "users:roles:assign"));
    options.AddPolicy("users:policies:read", policy => policy.RequireClaim("policy", "users:policies:read"));
    options.AddPolicy("users:policies:assign", policy => policy.RequireClaim("policy", "users:policies:assign"));
    options.AddPolicy("users:policies:remove", policy => policy.RequireClaim("policy", "users:policies:remove"));

    // Policies Management
    options.AddPolicy("policies:read", policy => policy.RequireClaim("policy", "policies:read"));
    options.AddPolicy("policies:create", policy => policy.RequireClaim("policy", "policies:create"));
    options.AddPolicy("policies:update", policy => policy.RequireClaim("policy", "policies:update"));
    options.AddPolicy("policies:delete", policy => policy.RequireClaim("policy", "policies:delete"));
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
