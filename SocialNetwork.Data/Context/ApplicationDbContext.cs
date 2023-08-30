using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SocialNetwork.Models.Db;
using SocialNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions <ApplicationDbContext> options) : base(options) {
            Database.EnsureCreated();
        }
    }
}
