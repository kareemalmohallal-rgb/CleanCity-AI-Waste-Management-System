using System.ComponentModel.DataAnnotations;

namespace CleanCity.MVC.Models
{
    public class EditDriverUserVm
    {
        public string UserId { get; set; } = default!;
        public string FullName { get; set; } = "—";

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public string UserName { get; set; } = default!;

        // اختياري — إذا تُرك فارغاً لا تتغير كلمة المرور
        [MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
        public string? NewPassword { get; set; }
    }
}