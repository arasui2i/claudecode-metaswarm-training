using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Account>()
            .HasQueryFilter(a => !a.IsDeleted);
        modelBuilder.Entity<Account>()
            .HasIndex(a => a.Name).IsUnique();
        modelBuilder.Entity<Account>()
            .Property(a => a.AnnualRevenue).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Contact>()
            .HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Contact>()
            .HasIndex(c => c.Email).IsUnique();
        modelBuilder.Entity<Lead>()
            .HasQueryFilter(l => !l.IsDeleted);
        modelBuilder.Entity<Lead>()
            .HasIndex(l => l.Email).IsUnique();
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Email).IsUnique();
        modelBuilder.Entity<Customer>()
            .Property(c => c.AnnualIncome).HasColumnType("decimal(18,2)");

        modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
        modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<Permission>()
            .HasIndex(p => p.ActionKey).IsUnique();

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var adminId = new Guid("00000000-0000-0000-0000-000000000001");
        var salesId = new Guid("00000000-0000-0000-0000-000000000002");
        var viewerId = new Guid("00000000-0000-0000-0000-000000000003");

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = adminId, Name = "Admin" },
            new Role { Id = salesId, Name = "Sales" },
            new Role { Id = viewerId, Name = "Viewer" }
        );

        var permCustomersView = new Guid("00000000-0000-0000-0001-000000000001");
        var permCustomersEdit = new Guid("00000000-0000-0000-0001-000000000002");
        var permCustomersDelete = new Guid("00000000-0000-0000-0001-000000000003");

        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = permCustomersView, Name = "View Customers", ActionKey = "customers.view" },
            new Permission { Id = permCustomersEdit, Name = "Edit Customers", ActionKey = "customers.edit" },
            new Permission { Id = permCustomersDelete, Name = "Delete Customers", ActionKey = "customers.delete" }
        );

        // Admin gets all permissions; Sales gets view+edit; Viewer gets view only
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = adminId, PermissionId = permCustomersView },
            new RolePermission { RoleId = adminId, PermissionId = permCustomersEdit },
            new RolePermission { RoleId = adminId, PermissionId = permCustomersDelete },
            new RolePermission { RoleId = salesId, PermissionId = permCustomersView },
            new RolePermission { RoleId = salesId, PermissionId = permCustomersEdit },
            new RolePermission { RoleId = viewerId, PermissionId = permCustomersView }
        );
    }
}
