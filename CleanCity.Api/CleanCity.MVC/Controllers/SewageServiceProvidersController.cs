using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.MVC.Controllers
{
    public class SewageServiceProvidersController : Controller
    {
        private readonly ISewageServiceProviderService _providers;

        public SewageServiceProvidersController(ISewageServiceProviderService providers)
        {
            _providers = providers;
        }

        // GET: /SewageServiceProviders
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var items = await _providers.GetAllAsync(ct);
            return View(items);
        }

        // GET: /SewageServiceProviders/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var item = await _providers.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            return View(item);
        }

        // GET: /SewageServiceProviders/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new SewageServiceProvider());
        }

        // POST: /SewageServiceProviders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SewageServiceProvider provider, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(provider);

            await _providers.CreateAsync(provider, ct);
            return RedirectToAction(nameof(Index));
        }

        // GET: /SewageServiceProviders/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var item = await _providers.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /SewageServiceProviders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SewageServiceProvider provider, CancellationToken ct)
        {
            if (id != provider.Id)
                return BadRequest("Id mismatch");

            if (!ModelState.IsValid)
                return View(provider);

            try
            {
                await _providers.UpdateAsync(provider, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(provider);
            }
        }

        // GET: /SewageServiceProviders/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var item = await _providers.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /SewageServiceProviders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                await _providers.DeleteAsync(id, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                var item = await _providers.GetByIdAsync(id, ct);
                if (item == null) return NotFound();

                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Delete", item);
            }
        }
    }
}