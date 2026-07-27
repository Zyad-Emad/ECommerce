using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
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

        public async Task<Result<bool>> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            return await userManager.FindByEmailAsync(email) is not null ? Result<bool>.Ok(true) : Result<bool>.Ok(false);
        }

        public async Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default)
        {
            var user =  await userManager.FindByEmailAsync(email);
            if (user == null) return Result<IdentityUserResult>.Fail(Error.NotFound("User Not Found", $"User With Email {email} Not Found"));
            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.Email!, user.UserName!));
        }

        public async Task<Result<AddressDto>> GetUserAddressByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await userManager.Users.Include(x => x.Address).FirstOrDefaultAsync(x => x.Email == email, ct);
            if (user?.Address == null) return Result<AddressDto>.Fail(Error.NotFound("Address Not Found", $"Address Of User With Email {email} Does Not Exist"));
            var address = user.Address;
            return Result<AddressDto>.Ok(new AddressDto()
            {
                FirstName = address.FirstName,
                LastName = address.LastName,
                City = address.City,
                Country = address.Country,
                Street = address.Street
            });
        }

        public async Task<Result<IReadOnlyList<string>>> GetUserRolesAsync(string email, CancellationToken ct = default)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user == null)
                return Result<IReadOnlyList<string>>.Fail(Error.NotFound("User Not Found", $"User With Email {email} Not Found"));

            var roles = (await userManager.GetRolesAsync(user)).ToList();
            return Result<IReadOnlyList<string>>.Ok(roles);
        }

        public async Task<Result<AddressDto>> UpdateOrInsertUserAddressAsync(string email, AddressDto addressDto, CancellationToken ct = default)
        {
            var user = await userManager.Users.Include(x => x.Address).FirstOrDefaultAsync(x => x.Email == email, ct);
            if(user.Address == null)
            {
                user.Address = new Address()
                {
                    FirstName = addressDto.FirstName,
                    LastName = addressDto.LastName,
                    City = addressDto.City,
                    Country = addressDto.Country,
                    Street = addressDto.Street
                };
            }
            else
            {
                user.Address.FirstName = addressDto.FirstName;
                user.Address.LastName = addressDto.LastName;
                user.Address.City = addressDto.City;
                user.Address.Country = addressDto.Country;
                user.Address.Street = addressDto.Street;
            }
            var res = await userManager.UpdateAsync(user);
            if (res.Succeeded) return Result<AddressDto>.Ok(addressDto);
            return Result<AddressDto>.Fail(Error.Failure("Failure", string.Join(';', res.Errors.Select(e => e.Description))));

        }
    }
}
