using FullCartApi.DTO;
using FullCartApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FullCartApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        
        public AuthController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            // Validate user credentials and generate JWT token
            var user = _userRepository.GetUserByUsernameAndPassword(loginModel.Username, loginModel.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = GenerateJwtToken(user.Role.RoleName);
            return Ok(new { token });
        }

        private string GenerateJwtToken(string role)
        {
            // Use a token generator library or create your own logic to generate JWT token

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, role),
                // Add other claims as needed (e.g., user ID, username, etc.)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://your-auth-server.com",
                audience: "https://your-api.com",
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Token expiration time
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("logout")]
        [Authorize] // Ensure the user is authenticated to log out
        public IActionResult Logout()
        {
            // Clear authentication token or cookie on the client side
            // ...

            return Ok(new { message = "Logout successful" });
        }
    }

}
