using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;

public class RolePolicyConfiguration : IEntityTypeConfiguration<RolePolicy> {
    public void Configure(EntityTypeBuilder<RolePolicy> builder) {
        builder.ToTable("roles_has_policies");

        builder.HasKey(rp => new { rp.RoleId, rp.PolicyId });

        builder.Property(rp => rp.RoleId).HasColumnName("role_id").HasColumnType("uuid");
        builder.Property(rp => rp.PolicyId).HasColumnName("policy_id").HasColumnType("uuid");

        var userId = Guid.Parse("a191243f-1149-4b19-a66c-96541dc2deff");
        var adminId = Guid.Parse("33ee7453-b06c-4959-9377-badf265ee52d");

        builder.HasData(
            // User policies
            new { RoleId = userId, PolicyId = Guid.Parse("2666478b-d202-412e-b166-392499b06c95") },
            new { RoleId = userId, PolicyId = Guid.Parse("d0f61701-de19-4e1e-bea9-dd434d4e982c") },
            new { RoleId = userId, PolicyId = Guid.Parse("d54a78a0-0387-43fd-9fe9-c1defca482e1") },

            // Admin policies
            new { RoleId = adminId, PolicyId = Guid.Parse("3fbed1fd-75e7-42a9-b017-8a2be84c81f1") },
            new { RoleId = adminId, PolicyId = Guid.Parse("9a308869-5e11-4480-916b-cef3908797dc") },
            new { RoleId = adminId, PolicyId = Guid.Parse("cdd60f4c-521f-4a45-8b87-1b84b69d49c1") },
            new { RoleId = adminId, PolicyId = Guid.Parse("321aa912-e34e-4f41-9dbf-f5f61a3951f2") },

            new { RoleId = adminId, PolicyId = Guid.Parse("a10b5c92-3e9b-4d8a-bf01-19d3f9b6f111") },
            new { RoleId = adminId, PolicyId = Guid.Parse("b21e5d93-4f7c-4d2e-a401-28a5f0c7e222") },
            new { RoleId = adminId, PolicyId = Guid.Parse("c32f6e94-5a8d-4d3f-b502-37c7f1d8f333") },
            new { RoleId = adminId, PolicyId = Guid.Parse("d43f7f95-6b9e-4e40-c603-46e8f2e9f444") },
            new { RoleId = adminId, PolicyId = Guid.Parse("e54f8096-7caf-4f51-d704-55f9f3faf555") },
            new { RoleId = adminId, PolicyId = Guid.Parse("f65f9197-8db0-4f62-e805-64faf4fbf666") },
            new { RoleId = adminId, PolicyId = Guid.Parse("067fa298-9ec1-4f73-f906-73fb05fc0777") },
            new { RoleId = adminId, PolicyId = Guid.Parse("178fb399-afd2-5074-0a07-82fc16fd1888") },

            new { RoleId = adminId, PolicyId = Guid.Parse("289fc49a-c0e3-6185-1b18-91fd27fe2999") },
            new { RoleId = adminId, PolicyId = Guid.Parse("39afd59b-d1f4-7296-2c29-a20e380f3aaa") },
            new { RoleId = adminId, PolicyId = Guid.Parse("4ab0e69c-e205-83a7-3d3a-b31f491045bb") },
            new { RoleId = adminId, PolicyId = Guid.Parse("5bc1f79d-f316-94b8-4e4b-c42f5a2156cc") },
            new { RoleId = adminId, PolicyId = Guid.Parse("6cd2089e-0427-a5c9-5f5c-d53f6b3267dd") },

            new { RoleId = adminId, PolicyId = Guid.Parse("7de3199f-1538-b6da-607d-e64f7c4378ee") },
            new { RoleId = adminId, PolicyId = Guid.Parse("8ef42aa0-2649-c7eb-718e-f75f8d5489ff") },
            new { RoleId = adminId, PolicyId = Guid.Parse("9ff53bb1-375a-d8fc-829f-08609e659a00") },
            new { RoleId = adminId, PolicyId = Guid.Parse("a1064cc2-486b-e90d-93a0-1971af76ab11") }
        );
    }
}
