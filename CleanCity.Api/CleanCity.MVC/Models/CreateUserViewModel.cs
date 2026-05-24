using System.ComponentModel.DataAnnotations;

namespace CleanCity.MVC.Models
{
    public class CreateUserViewModel
    {
        
        public int? DriverId { get; set; }

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [StringLength(50, ErrorMessage = "اسم المستخدم يجب ألا يتجاوز 50 حرفًا")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة السر مطلوبة")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "كلمة السر يجب أن تكون 6 أحرف على الأقل")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "تأكيد كلمة السر مطلوب")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "كلمتا السر غير متطابقتين")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
