using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }
        // Login
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto , CancellationToken ct)
            => ToActionResult(await authenticationService.LoginAsync(loginDto , ct));
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto , CancellationToken ct)
            => ToActionResult(await authenticationService.RegisterAsync(registerDto , ct));

        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmail([FromQuery] string email , CancellationToken ct)
            => ToActionResult(await authenticationService.CheckEmailExistsAsync(email , ct));

        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken ct)
            => ToActionResult(await authenticationService.GetCurrentUserAsync(GetEmailFromToken(), ct));
        
        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress(CancellationToken ct)
            => ToActionResult(await authenticationService.GetUserAddressAsync(GetEmailFromToken(), ct));
        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto, CancellationToken ct)
            => ToActionResult(await authenticationService.UpSertUserAddressAsync(GetEmailFromToken(), addressDto, ct));
    
        
    }
}
