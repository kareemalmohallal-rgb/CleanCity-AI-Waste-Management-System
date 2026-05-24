using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.MVC.Controllers
{
    public class ReportAssignmentsController : Controller
    {
        private readonly IReportAssignmentService _assignments;

        public ReportAssignmentsController(IReportAssignmentService assignments)
        {
            _assignments = assignments;
        }

        // GET: /ReportAssignments
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var items = await _assignments.GetAllAsync(ct);
            return View(items);
        }

        // GET: /ReportAssignments/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var item = await _assignments.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            return View(item);
        }

        // GET: /ReportAssignments/ByReport/10
        [HttpGet]
        public async Task<IActionResult> ByReport(int reportId, CancellationToken ct)
        {
            var items = await _assignments.GetByReportIdAsync(reportId, ct);
            ViewBag.ReportId = reportId;
            return View(items); // View: Views/ReportAssignments/ByReport.cshtml
        }

        // GET: /ReportAssignments/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ReportAssignment());
        }

        // POST: /ReportAssignments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReportAssignment assignment, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(assignment);

            await _assignments.CreateAsync(assignment, ct);
            return RedirectToAction(nameof(Index));
        }

        // GET: /ReportAssignments/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var item = await _assignments.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /ReportAssignments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReportAssignment assignment, CancellationToken ct)
        {
            if (id != assignment.Id)
                return BadRequest("Id mismatch.");

            if (!ModelState.IsValid)
                return View(assignment);

            try
            {
                await _assignments.UpdateAsync(assignment, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(assignment);
            }
        }

        // GET: /ReportAssignments/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var item = await _assignments.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /ReportAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                await _assignments.DeleteAsync(id, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                var item = await _assignments.GetByIdAsync(id, ct);
                if (item == null) return NotFound();

                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Delete", item);
            }
        }
    }
}