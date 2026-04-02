using Microsoft.AspNetCore.Mvc;
using BugTracker.Web.Models;

namespace BugTracker.Web.Controllers
{
    public class HomeController : Controller
    {
        private static List<Bug> _bugs = new List<Bug>
        {
            new Bug { Id = 1, Title = "Login page crashes on empty input", Description = "App throws NullReferenceException when submitting empty login form. Reproducible every time.", ReportedBy = "alice", AssignedTo = "bob", Status = BugStatus.Open, Priority = BugPriority.Critical, CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new Bug { Id = 2, Title = "Dashboard loads slowly", Description = "Dashboard takes 8+ seconds to load on first visit. Affects all users.", ReportedBy = "bob", AssignedTo = "carol", Status = BugStatus.InProgress, Priority = BugPriority.High, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new Bug { Id = 3, Title = "Typo in footer text", Description = "Footer says 'Copywright' instead of 'Copyright'.", ReportedBy = "carol", AssignedTo = "", Status = BugStatus.Open, Priority = BugPriority.Low, CreatedAt = DateTime.UtcNow.AddHours(-5) },
            new Bug { Id = 4, Title = "Export to CSV broken", Description = "CSV export produces empty file for date ranges over 30 days.", ReportedBy = "dave", AssignedTo = "alice", Status = BugStatus.Resolved, Priority = BugPriority.Medium, CreatedAt = DateTime.UtcNow.AddDays(-5), ResolvedAt = DateTime.UtcNow.AddDays(-1) },
            new Bug { Id = 5, Title = "Email notifications not sending", Description = "Users not receiving password reset emails. SMTP config issue suspected.", ReportedBy = "alice", AssignedTo = "dave", Status = BugStatus.InProgress, Priority = BugPriority.High, CreatedAt = DateTime.UtcNow.AddDays(-2) }
        };

        private static List<Project> _projects = new List<Project>
        {
            new Project { Id = 1, Name = "Frontend", Description = "UI and UX related bugs", CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new Project { Id = 2, Name = "Backend API", Description = "Server-side and API bugs", CreatedAt = DateTime.UtcNow.AddDays(-8) },
            new Project { Id = 3, Name = "DevOps", Description = "Infrastructure and deployment bugs", CreatedAt = DateTime.UtcNow.AddDays(-5) }
        };

        private static int _nextBugId = 6;
        private static int _nextProjectId = 4;

        // GET: / — Dashboard
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            ViewData["Active"] = "Dashboard";
            return View(_bugs);
        }

        // GET: /bugs — All Bugs
        [HttpGet("/bugs")]
        public IActionResult Bugs(string? status, string? priority, string? search)
        {
            ViewData["Title"] = "All Bugs";
            ViewData["Active"] = "Bugs";
            ViewData["StatusFilter"] = status;
            ViewData["PriorityFilter"] = priority;
            ViewData["Search"] = search;

            var filtered = _bugs.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<BugStatus>(status, out var s))
                filtered = filtered.Where(b => b.Status == s);
            if (!string.IsNullOrWhiteSpace(priority) && Enum.TryParse<BugPriority>(priority, out var p))
                filtered = filtered.Where(b => b.Priority == p);
            if (!string.IsNullOrWhiteSpace(search))
                filtered = filtered.Where(b => b.Title.Contains(search, StringComparison.OrdinalIgnoreCase) || b.Description.Contains(search, StringComparison.OrdinalIgnoreCase));

            return View(filtered.OrderByDescending(b => b.CreatedAt).ToList());
        }

        // GET: /bugs/create
        [HttpGet("/bugs/create")]
        public IActionResult CreateBug()
        {
            ViewData["Title"] = "Report New Bug";
            ViewData["Active"] = "Bugs";
            ViewData["Projects"] = _projects;
            return View();
        }

        // POST: /bugs/create
        [HttpPost("/bugs/create")]
        public IActionResult CreateBug(string title, string description, string reportedBy, string assignedTo, BugPriority priority, int? projectId)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                _bugs.Add(new Bug
                {
                    Id = _nextBugId++,
                    Title = title,
                    Description = description,
                    ReportedBy = reportedBy,
                    AssignedTo = assignedTo,
                    Priority = priority,
                    Status = BugStatus.Open,
                    CreatedAt = DateTime.UtcNow,
                    ProjectId = projectId
                });
            }
            return RedirectToAction("Bugs");
        }

        // GET: /bugs/{id}
        [HttpGet("/bugs/{id:int}")]
        public IActionResult BugDetail(int id)
        {
            var bug = _bugs.FirstOrDefault(b => b.Id == id);
            if (bug == null) return NotFound();
            ViewData["Title"] = $"Bug #{id}";
            ViewData["Active"] = "Bugs";
            ViewData["Projects"] = _projects;
            return View(bug);
        }

        // GET: /bugs/{id}/edit
        [HttpGet("/bugs/{id:int}/edit")]
        public IActionResult EditBug(int id)
        {
            var bug = _bugs.FirstOrDefault(b => b.Id == id);
            if (bug == null) return NotFound();
            ViewData["Title"] = $"Edit Bug #{id}";
            ViewData["Active"] = "Bugs";
            ViewData["Projects"] = _projects;
            return View(bug);
        }

        // POST: /bugs/{id}/edit
        [HttpPost("/bugs/{id:int}/edit")]
        public IActionResult EditBug(int id, string title, string description, string reportedBy, string assignedTo, BugPriority priority, BugStatus status, int? projectId)
        {
            var bug = _bugs.FirstOrDefault(b => b.Id == id);
            if (bug != null)
            {
                bug.Title = title;
                bug.Description = description;
                bug.ReportedBy = reportedBy;
                bug.AssignedTo = assignedTo;
                bug.Priority = priority;
                bug.Status = status;
                bug.ProjectId = projectId;
                if (status == BugStatus.Resolved && bug.ResolvedAt == null)
                    bug.ResolvedAt = DateTime.UtcNow;
            }
            return RedirectToAction("BugDetail", new { id });
        }

        // POST: /bugs/{id}/resolve
        [HttpPost("/bugs/{id:int}/resolve")]
        public IActionResult Resolve(int id)
        {
            var bug = _bugs.FirstOrDefault(b => b.Id == id);
            if (bug != null) { bug.Status = BugStatus.Resolved; bug.ResolvedAt = DateTime.UtcNow; }
            return RedirectToAction("BugDetail", new { id });
        }

        // POST: /bugs/{id}/close
        [HttpPost("/bugs/{id:int}/close")]
        public IActionResult Close(int id)
        {
            var bug = _bugs.FirstOrDefault(b => b.Id == id);
            if (bug != null) bug.Status = BugStatus.Closed;
            return RedirectToAction("BugDetail", new { id });
        }

        // POST: /bugs/{id}/delete
        [HttpPost("/bugs/{id:int}/delete")]
        public IActionResult Delete(int id)
        {
            _bugs.RemoveAll(b => b.Id == id);
            return RedirectToAction("Bugs");
        }

        // GET: /projects
        [HttpGet("/projects")]
        public IActionResult Projects()
        {
            ViewData["Title"] = "Projects";
            ViewData["Active"] = "Projects";
            return View(_projects.Select(p => new ProjectViewModel
            {
                Project = p,
                BugCount = _bugs.Count(b => b.ProjectId == p.Id),
                OpenCount = _bugs.Count(b => b.ProjectId == p.Id && b.Status == BugStatus.Open)
            }).ToList());
        }

        // POST: /projects/create
        [HttpPost("/projects/create")]
        public IActionResult CreateProject(string name, string description)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _projects.Add(new Project
                {
                    Id = _nextProjectId++,
                    Name = name,
                    Description = description,
                    CreatedAt = DateTime.UtcNow
                });
            }
            return RedirectToAction("Projects");
        }

        // POST: /projects/{id}/delete
        [HttpPost("/projects/{id:int}/delete")]
        public IActionResult DeleteProject(int id)
        {
            _projects.RemoveAll(p => p.Id == id);
            return RedirectToAction("Projects");
        }

        // GET: /health
        [HttpGet("/health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", app = "BugTracker", version = "1.0.0", timestamp = DateTime.UtcNow });
        }

        // GET: /metrics-info
        [HttpGet("/metrics-info")]
        public IActionResult MetricsInfo()
        {
            return Ok(new {
                totalBugs = _bugs.Count,
                openBugs = _bugs.Count(b => b.Status == BugStatus.Open),
                inProgressBugs = _bugs.Count(b => b.Status == BugStatus.InProgress),
                resolvedBugs = _bugs.Count(b => b.Status == BugStatus.Resolved),
                closedBugs = _bugs.Count(b => b.Status == BugStatus.Closed),
                criticalBugs = _bugs.Count(b => b.Priority == BugPriority.Critical)
            });
        }
    }
}
