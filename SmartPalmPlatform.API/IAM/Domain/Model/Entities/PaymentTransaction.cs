using SmartPalmPlatform.API.IAM.Domain.Model.Enums;

namespace SmartPalmPlatform.API.IAM.Domain.Model.Entities;

public class PaymentTransaction
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string PlanName { get; private set; } = null!;
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public decimal Amount { get; private set; }
    public string? TransactionId { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime ProcessedAt { get; private set; }

    public PaymentTransaction(int userId, string planName, DateTime periodStart, DateTime periodEnd, decimal amount)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be greater than zero.");
        if (string.IsNullOrWhiteSpace(planName))
            throw new ArgumentException("PlanName cannot be empty.");
        if (periodStart >= periodEnd)
            throw new ArgumentException("PeriodStart must be before PeriodEnd.");
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        UserId = userId;
        PlanName = planName;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
        Amount = amount;
        Status = PaymentStatus.Pending;
        ProcessedAt = DateTime.UtcNow;
    }

    private PaymentTransaction() { }

    public PaymentTransaction Complete(string transactionId)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending transactions can be completed.");
        if (string.IsNullOrWhiteSpace(transactionId))
            throw new ArgumentException("Transaction id cannot be empty.");

        TransactionId = transactionId;
        Status = PaymentStatus.Completed;
        return this;
    }

    public PaymentTransaction Fail()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending transactions can be marked as failed.");
        Status = PaymentStatus.Failed;
        return this;
    }

    public PaymentTransaction Refund()
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("Only completed transactions can be refunded.");
        Status = PaymentStatus.Refunded;
        return this;
    }
}
