using Microsoft.AspNetCore.Mvc;
using UserManagement.Dtos;
using UserManagement.Services;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        /// <summary>Sign in with email + password and get a fake JWT-shaped token.</summary>
        [HttpPost("sign-in")]
        public async Task<ActionResult<AuthResponse>> SignIn([FromBody] SignInRequest request)
        {
            var result = await _auth.SignInAsync(request);
            if (result == null) return Unauthorized(new { message = "Invalid credentials" });
            return Ok(result);
        }
    }
}
