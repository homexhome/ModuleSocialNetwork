using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialNetwork.Models.Db;

namespace SocialNetwork.Web.Views.AccountManager
{
    public class UserListModel : PageModel
    {
        public List<User> UserList { get; set; }
    }
}
