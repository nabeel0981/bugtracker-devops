namespace BugTracker.Web.Models
{
    public class ProjectViewModel
    {
        public Project Project { get; set; } = new Project();
        public int BugCount { get; set; }
        public int OpenCount { get; set; }
    }
}
