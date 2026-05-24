using System.ComponentModel.DataAnnotations;

namespace CleanCity.MVC.Models
{
    public class LoginViewModel
    {
        public LoginInputModel Input { get; set; } = new();
    }

    public class LoginInputModel
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}