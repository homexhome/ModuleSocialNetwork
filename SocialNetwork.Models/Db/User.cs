﻿using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Models.Db
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
        public string About { get; set; }

        public string GetFullName() { return String.Join(' ', FirstName, LastName); }

        public User() {
            Image = "https://thispersondoesnotexist.com";
            Status = "OK";
            About = "About me";
        }

    }
}
