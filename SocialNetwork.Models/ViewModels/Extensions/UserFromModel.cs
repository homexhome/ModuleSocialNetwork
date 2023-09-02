using SocialNetwork.Models.Db;
using SocialNetwork.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Models.ViewModels.Extensions
{
    public static class UserFromModel
    {
        public static User Convert(this User user, UserEditViewModel userEditViewModel) {
            user.Image = userEditViewModel.Image;
            user.LastName = userEditViewModel.LastName;
            user.MiddleName = userEditViewModel.MiddleName;
            user.FirstName = userEditViewModel.FirstName;
            user.Email = userEditViewModel.Email;
            user.BirthDate = userEditViewModel.BirthDate;
            user.UserName = userEditViewModel.UserName;
            user.Status = userEditViewModel.Status;
            user.About = userEditViewModel.About;

            return user;
        } 
    }
}
