using SocialNetwork.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Models.ViewModels.Account
{
    public class UserWithFriendExt : User
    {
        public bool IsFriendWithCurrent { get; set; }
    }
}
