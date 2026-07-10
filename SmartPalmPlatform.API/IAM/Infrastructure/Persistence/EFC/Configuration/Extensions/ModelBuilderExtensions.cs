using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Domain.Model.Entities;

namespace SmartPalmPlatform.API.IAM.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyIamConfiguration(this ModelBuilder builder)
    {
        ConfigureUser(builder);
        ConfigureSubscription(builder);
        ConfigurePaymentTransaction(builder);
    }

    private static void ConfigureUser(ModelBuilder builder)
    {
        builder.Entity<User>().HasKey(u => u.Id);
        builder.Entity<User>().Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<User>().Property(u => u.Username).IsRequired();
        builder.Entity<User>().Property(u => u.PasswordHash).IsRequired();
        builder.Entity<User>().Property(u => u.Email).IsRequired();
        builder.Entity<User>().Property(u => u.FullName).IsRequired();
        builder.Entity<User>().Property(u => u.Role).IsRequired();
        builder.Entity<User>().Property(u => u.Status).IsRequired();
    }

    private static void ConfigureSubscription(ModelBuilder builder)
    {
        builder.Entity<Subscription>().HasKey(s => s.Id);
        builder.Entity<Subscription>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Subscription>().Property(s => s.UserId).IsRequired();
        builder.Entity<Subscription>().Property(s => s.PlanType).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Entity<Subscription>().Property(s => s.PlanName).IsRequired().HasMaxLength(100);
        builder.Entity<Subscription>().Property(s => s.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Entity<Subscription>().Property(s => s.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Entity<Subscription>().Property(s => s.StartDate).IsRequired();
        builder.Entity<Subscription>().Property(s => s.EndDate).IsRequired();
        builder.Entity<Subscription>().Property(s => s.BillingCycle).IsRequired().HasConversion<string>().HasMaxLength(10);
        builder.Entity<Subscription>().Property(s => s.CreatedAt).IsRequired();
    }

    private static void ConfigurePaymentTransaction(ModelBuilder builder)
    {
        builder.Entity<PaymentTransaction>().HasKey(t => t.Id);
        builder.Entity<PaymentTransaction>().Property(t => t.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<PaymentTransaction>().Property(t => t.UserId).IsRequired();
        builder.Entity<PaymentTransaction>().Property(t => t.PlanName).IsRequired().HasMaxLength(100);
        builder.Entity<PaymentTransaction>().Property(t => t.PeriodStart).IsRequired();
        builder.Entity<PaymentTransaction>().Property(t => t.PeriodEnd).IsRequired();
        builder.Entity<PaymentTransaction>().Property(t => t.Amount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Entity<PaymentTransaction>().Property(t => t.TransactionId).IsRequired(false).HasMaxLength(100);
        builder.Entity<PaymentTransaction>().Property(t => t.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Entity<PaymentTransaction>().Property(t => t.ProcessedAt).IsRequired();
    }
}
