using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;

namespace SmartPalmPlatform.API.IAM.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyIamConfiguration(this ModelBuilder builder)
    {
        // IAM Context

        builder.Entity<User>().ToTable("users");
        builder.Entity<User>().HasKey(u => u.Id);
        builder.Entity<User>().Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        builder.Entity<User>().Property(u => u.Username).IsRequired();
        builder.Entity<User>().Property(u => u.PasswordHash).IsRequired();
        builder.Entity<User>().Property(u => u.Role).IsRequired().HasConversion<string>();
        builder.Entity<User>().Property(u => u.SubscriptionId);
    }
}