namespace BugTracker.Web.Models
{
    public class Bug
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ReportedBy { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public BugStatus Status { get; set; } = BugStatus.Open;
        public BugPriority Priority { get; set; } = BugPriority.Medium;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
        public int? ProjectId { get; set; }
    }

    public enum BugStatus
    {
        Open,
        InProgress,
        Resolved,
        Closed
    }

    public enum BugPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}
