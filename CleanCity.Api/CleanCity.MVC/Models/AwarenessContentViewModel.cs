// CleanCity.MVC/Models/AwarenessContentViewModel.cs

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CleanCity.MVC.Models
{
    public class AwarenessContentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        [Display(Name = "العنوان")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "المحتوى مطلوب")]
        [Display(Name = "محتوى المقال")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "صورة المقال")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}