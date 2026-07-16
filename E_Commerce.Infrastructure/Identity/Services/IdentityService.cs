using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Identity.Services
{
    internal class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return Result<bool>.Fail(Error.NotFound("User Not Found", $"User With Email {email} Not Found"));
            return Result<bool>.Ok(await userManager.CheckPasswordAsync(user, password)); 
        }

        public async Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var user = new ApplicationUser()
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                DisplayName = registerDto.DisplayName,
                PhoneNumber = registerDto.PhoneNumber
            };
            var res = await userManager.CreateAsync(user, registerDto.Password);
            if (!res.Succeeded)
            {
                var errors = res.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
                return Result<IdentityUserResult>.Fail(errors);
            }
            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.Email, user.UserName));
        }

        public async Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default)
        {
            var user =  await userManager.FindByEmailAsync(email);
            if (user == null) return Result<IdentityUserResult>.Fail(Error.NotFound("User Not Found", $"User With Email {email} Not Found"));
            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.Email!, user.UserName!));
        }

        public async Task<Result<IReadOnlyList<string>>> GetUserRolesAsync(string email, CancellationToken ct = default)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user == null)
                return Result<IReadOnlyList<string>>.Fail(Error.NotFound("User Not Found", $"User With Email {email} Not Found"));

            var roles = (await userManager.GetRolesAsync(user)).ToList();
            return Result<IReadOnlyList<string>>.Ok(roles);
        }
    }
}
