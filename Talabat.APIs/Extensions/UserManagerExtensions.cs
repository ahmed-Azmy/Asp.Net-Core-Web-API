﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Talabat.DAL.Entities.Identity;

namespace Talabat.APIs.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<AppUser> FindUserAndAddressByEmailAsync(this UserManager<AppUser> userManager , ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user =await userManager.Users.Include(U => U.Address).SingleOrDefaultAsync(U => U.Email == email);

            return user;
        }
    }
}
