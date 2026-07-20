using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityService identityService;
        private readonly ITokenService tokenService;

        public AuthenticationService(IIdentityService identityService , ITokenService tokenService)
        {
            this.identityService = identityService;
            this.tokenService = tokenService;
        }

        public async Task<Result<bool>> CheckEmailExistsAsync(string email, CancellationToken ct = default)
            => await identityService.EmailExistsAsync(email, ct);

        public async Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken ct = default)
        {
            var userResult = await identityService.FindUserByEmailAsync(email, ct);

            var user = userResult.data;

            var roleResult = await identityService.GetUserRolesAsync(email, ct);
            
            var token = tokenService.CreateToken(user.Id, user.Email, user.UserName, roleResult.data);
            return Result<UserDto>.Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = email,
                Token = token
            });
        }

        public async Task<Result<AddressDto>> GetUserAddressAsync(string email, CancellationToken ct = default)
        {
            return await identityService.GetUserAddressByEmailAsync(email, ct);
        }

        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            var userResult = await identityService.FindUserByEmailAsync(loginDto.Email , ct);
            if (!userResult.IsSuccess)
                return Result<UserDto>.Fail(userResult.Errors);

            var passwordResult = await identityService.CheckPasswordAsync(loginDto.Email, loginDto.Password, ct);

            if (!passwordResult.IsSuccess)
                return Result<UserDto>.Fail(passwordResult.Errors);
            if (!passwordResult.data)
                return Result<UserDto>.Fail(Error.Unauthorized("Invalid Email Or Password"));

            var user = userResult.data;
            var rolesResult = await identityService.GetUserRolesAsync(user.Email, ct);
            var roles = rolesResult.data;
            var token = tokenService.CreateToken(user.Id, user.Email, user.UserName, roles);

            return Result<UserDto>.Ok(new UserDto()
            {
                Email = loginDto.Email,
                DisplayName = userResult.data.DisplayName,
                Token = token
            });

        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var userRes = await identityService.CreateUserAsync(registerDto, ct);
            if (!userRes.IsSuccess) return Result<UserDto>.Fail(userRes.Errors);
            
            var user = userRes.data;
            var rolesResult = await identityService.GetUserRolesAsync(user.Email, ct);
            var roles = rolesResult.data;
            var token = tokenService.CreateToken(user.Id, user.Email, user.UserName, roles);

            return Result<UserDto>.Ok(new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = token
            });
        }

        public async Task<Result<AddressDto>> UpSertUserAddressAsync(string email, AddressDto addressDto, CancellationToken ct = default)
        {
            return await identityService.UpdateOrInsertUserAddressAsync(email, addressDto, ct);
        }
    }
}
