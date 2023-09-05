using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.ViewModels.Account
{
    public class RegisterViewModel
    {

        [Required]
        [Display(Name = "Имя", Prompt = "Введите имя")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Фамилия", Prompt = "Введите фамилию")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email", Prompt = "Введите Email")]
        public string EmailReg { get; set; }

        [Required]
        [Display(Name = "Год", Prompt = "Год")]
        public int? Year { get; set; } = 2008;

        [Required]
        [Display(Name = "День", Prompt = "День")]
        public int? Date { get; set; }

        [Required]
        [Display(Name = "Месяц", Prompt = "Месяц")]
        public int? Month { get; set; } = 1;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль", Prompt = "Введите пароль")]
        [StringLength(100, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 5)]
        public string PasswordReg { get; set; }

        [Required]
        [Compare("PasswordReg", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль", Prompt = "Введите пароль еще раз")]
        public string PasswordConfirm { get; set; }

        [Required]
        [Display(Name = "Никнейм", Prompt = "Введите никнейм")]
        public string Login { get; set; }
    }
}
