using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.DAL.Entities.Identity;

namespace Talabat.DAL.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName = "Ahmed Azmy",
                    UserName = "ahmedazmy",
                    Email = "ahmedazmy@yahoo.com",
                    PhoneNumber = "01042698748",
                    Address = new Address()
                    {
                        FirstName = "Ahmed",
                        LastName = "Azmy",
                        Country = "Egypt",
                        City = "Giza",
                        Street = "10 TH St."
                    }

                };

                await userManager.CreateAsync(user, "P@ssw0rd");
            }
        }
    }
}
