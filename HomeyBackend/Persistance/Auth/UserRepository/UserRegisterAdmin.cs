﻿using HomeyBackend.Core.Models;
using Microsoft.AspNetCore.Identity;
using HomeyBackend.Persistance;

namespace HomeyBackend.Persistance.Auth.UserRepository
{
    public class UserRegisterAdminRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Phone { get; set; } = "";
       
    }
    public partial class UserService
    {
        public async Task<AppResponse<bool>> UserRegisterAdminAsync(UserRegisterRequest request)
        {
            var user = new UserInfo()
            {
                UserName = request.Email,
                Email = request.Email,
                UserPhoto="not set yet we will set default later",
                PhoneNumber=request.Phone

            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                if (!await roleManager.RoleExistsAsync(UserRole.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRole.Admin));
                if (!await roleManager.RoleExistsAsync(UserRole.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRole.User));

                if (await roleManager.RoleExistsAsync(UserRole.Admin))
                {
                    await userManager.AddToRoleAsync(user, UserRole.Admin);
                }
                return new AppResponse<bool>().SetSuccessResponse(true);
            }
            else
            {
                return new AppResponse<bool>().SetErrorResponse(GetRegisterAdminErrors(result));
            }
        }

        private Dictionary<string, string[]> GetRegisterAdminErrors(IdentityResult result)
        {
            var errorDictionary = new Dictionary<string, string[]>(1);

            foreach (var error in result.Errors)
            {
                string[] newDescriptions;

                if (errorDictionary.TryGetValue(error.Code, out var descriptions))
                {
                    newDescriptions = new string[descriptions.Length + 1];
                    Array.Copy(descriptions, newDescriptions, descriptions.Length);
                    newDescriptions[descriptions.Length] = error.Description;
                }
                else
                {
                    newDescriptions = [error.Description];
                }

                errorDictionary[error.Code] = newDescriptions;
            }

            return errorDictionary;
        }

    }
}
