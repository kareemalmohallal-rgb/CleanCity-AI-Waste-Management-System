
// CleanCity.MVC/Controllers/AwarenessContentsController.cs
using CleanCity.Application.Interfaces.Services;
using CleanCity.Domain.Entities;
using CleanCity.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleanCity.MVC.Controllers
{
    public class AwarenessContentsController : Controller
    {
        private readonly IAwarenessContentService _contents;
        private readonly IWebHostEnvironment _env;

        public AwarenessContentsController(
            IAwarenessContentService contents,
            IWebHostEnvironment env)
        {
            _contents = contents;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var items = await _contents.GetAllAsync(ct);
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var item = await _contents.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AwarenessContentViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AwarenessContentViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(vm);

            string? imageUrl = null;

            if (vm.ImageFile != null && vm.ImageFile.Length > 0)
            {
                imageUrl = await SaveImageAsync(vm.ImageFile);
            }

            var entity = new AwarenessContent
            {
                Title = vm.Title,
                Content = vm.Content,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.Now
            };

            await _contents.CreateAsync(entity, ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var item = await _contents.GetByIdAsync(id, ct);
            if (item == null) return NotFound();

            var vm = new AwarenessContentViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Content = item.Content,
                ExistingImageUrl = item.ImageUrl,
                CreatedAt = item.CreatedAt
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AwarenessContentViewModel vm, CancellationToken ct)
        {
            if (id != vm.Id)
                return BadRequest("Id mismatch.");

            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                string? imageUrl = vm.ExistingImageUrl;

                if (vm.ImageFile != null && vm.ImageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(vm.ExistingImageUrl))
                        DeleteImage(vm.ExistingImageUrl);

                    imageUrl = await SaveImageAsync(vm.ImageFile);
                }

                var entity = new AwarenessContent
                {
                    Id = vm.Id,
                    Title = vm.Title,
                    Content = vm.Content,
                    ImageUrl = imageUrl,
                    CreatedAt = vm.CreatedAt
                };

                await _contents.UpdateAsync(entity, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var item = await _contents.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                var item = await _contents.GetByIdAsync(id, ct);

                await _contents.DeleteAsync(id, ct);

                if (item != null && !string.IsNullOrEmpty(item.ImageUrl))
                    DeleteImage(item.ImageUrl);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                var item = await _contents.GetByIdAsync(id, ct);
                if (item == null) return NotFound();

                ModelState.AddModelError(string.Empty,
                    "لا يمكن حذف هذا المحتوى لأنه مرتبط بإشعارات موجودة.");
                return View("Delete", item);
            }
            catch (InvalidOperationException ex)
            {
                var item = await _contents.GetByIdAsync(id, ct);
                if (item == null) return NotFound();

                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Delete", item);
            }
        }

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "awareness");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/awareness/{uniqueName}";
        }

        private void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(_env.WebRootPath, relativePath);

            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}