namespace Gao.Models;

/// <summary>
/// Represents suspicious activity detected in the system
/// </summary>
public class SuspiciousActivity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal SeverityScore { get; set; }
    public DateTime DetectedAt { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    public SuspiciousActivity()
    {
        DetectedAt = DateTime.UtcNow;
    }

    public SuspiciousActivity(string userId, string activityType, string description, decimal severityScore)
    {
        UserId = userId;
        ActivityType = activityType;
        Description = description;
        SeverityScore = severityScore;
        DetectedAt = DateTime.UtcNow;
    }
}
