using SmartPalmPlatform.API.IAM.Domain.Model.Enums;

namespace SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;

public class Subscription
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public PlanType PlanType { get; private set; }
    public string PlanName { get; private set; } = null!;
    public decimal Price { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public BillingCycle BillingCycle { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Subscription(
        int userId,
        PlanType planType,
        string planName,
        decimal price,
        BillingCycle billingCycle,
        DateTime startDate,
        DateTime endDate,
        SubscriptionStatus initialStatus = SubscriptionStatus.Pending)
    {
        if (userId <= 0)
            throw new ArgumentException("userId must be greater than zero.");
        if (string.IsNullOrWhiteSpace(planName))
            throw new ArgumentException("planName cannot be empty.");
        if (price < 0)
            throw new ArgumentException("price cannot be negative.");
        if (startDate >= endDate)
            throw new ArgumentException("startDate must be before endDate.");

        UserId = userId;
        PlanType = planType;
        PlanName = planName;
        Price = price;
        Status = initialStatus;
        StartDate = startDate;
        EndDate = endDate;
        BillingCycle = billingCycle;
        CreatedAt = DateTime.UtcNow;
    }

    public Subscription Activate()
    {
        if (Status != SubscriptionStatus.Pending)
            throw new InvalidOperationException("Only pending subscriptions can be activated.");
        Status = SubscriptionStatus.Active;
        return this;
    }

    private Subscription() { }

    public Subscription Cancel()
    {
        if (Status != SubscriptionStatus.Active)
            throw new InvalidOperationException("Only active subscriptions can be cancelled.");
        Status = SubscriptionStatus.Cancelled;
        return this;
    }

    public Subscription Renew(DateTime newEndDate)
    {
        if (Status != SubscriptionStatus.Active && Status != SubscriptionStatus.Expired)
            throw new InvalidOperationException("Only active or expired subscriptions can be renewed.");

        if (newEndDate <= EndDate)
            throw new ArgumentException("newEndDate must be after current endDate.");

        Status = SubscriptionStatus.Active;
        EndDate = newEndDate;
        return this;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > EndDate;
    }

    public Subscription MarkExpired()
    {
        Status = SubscriptionStatus.Expired;
        return this;
    }
}
