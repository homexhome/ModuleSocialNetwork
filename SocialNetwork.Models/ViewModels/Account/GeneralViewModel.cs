namespace SocialNetwork.Models.ViewModels.Account
{
    public class GeneralViewModel
    {
        public RegisterViewModel RegisterView { get; set; }

        public LoginViewModel LoginView { get; set; }


        public GeneralViewModel()
        {
            RegisterView = new RegisterViewModel();
            LoginView = new LoginViewModel();
        }
    }
}
