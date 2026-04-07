using Microsoft.AspNetCore.Mvc;
using Student_Api_Project.Model;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;
using Student_Api_Project.DTOs.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
namespace Student_Api_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var student = DataSimulation.StudentDataSimulation.Students.FirstOrDefault(s => s.Email == request.Email);
            if (student == null)
            {
                return Unauthorized(new { message = "Invalid Credentials" });
            }

            var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, student.PasswordHash);
            if (!isValidPassword)
            {
                return Unauthorized(new { message = "Invalid Credentials" });
            }

            var Claims = new[]
            {
                new Claim( ClaimTypes.NameIdentifier , student.Id.ToString()),
                new Claim(ClaimTypes.Email, student.Email),
                new Claim(ClaimTypes.Role, student.Role),

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THIS_IS_A_VERY_SECRET_KEY_123456"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "Student_Api_Project",
                audience: "StudentApiUser",
                claims: Claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
                );
            var AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var RefreshToken = GenerateRefreshToken();

            student.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(RefreshToken);
            student.RefreshTokenExpiresAt = DateTime.Now.AddDays(7);
            student.RefreshTonkenRevokedAt = null;

            return Ok(new
            {
                AccessToken,
                RefreshToken
            }
            );


        }
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);

        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            var student = DataSimulation.StudentDataSimulation.Students.FirstOrDefault(s => s.Email == request.Email);
            if (student == null)
            {
                return Unauthorized(new { message = "Invalid access token." });
            }

            if (student.RefreshTonkenRevokedAt != null)
            {
                return Unauthorized(new { message = "Invalid access token." });
            }


            if (student.RefreshTokenExpiresAt == null || student.RefreshTokenExpiresAt < DateTime.Now)
            {
                return Unauthorized(new { message = "Invalid refresh token." });
            }
            bool isValidRefreshToken = BCrypt.Net.BCrypt.Verify(request.RefreshToken, student.RefreshTokenHash);
            if (!isValidRefreshToken)
            {
                return Unauthorized(new { message = "Invalid refresh token." });
            }
            var claims = new[]
            {
              new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
              new Claim(ClaimTypes.Email, student.Email),
              new Claim(ClaimTypes.Role, student.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THIS_IS_A_VERY_SECRET_KEY_123456"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: "Student_Api_Project",
                audience: "StudentApiUser",
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(30),
               signingCredentials: creds
              );
            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
            var newRefreshToken = GenerateRefreshToken();
            student.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
            student.RefreshTokenExpiresAt = DateTime.Now.AddDays(7);
            student.RefreshTonkenRevokedAt = null;
            return Ok(new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        [HttpPost("Logout")]
        public IActionResult Logout([FromBody] LogoutRequest request)
        {
            var student = DataSimulation.StudentDataSimulation.Students.FirstOrDefault(s => s.Email == request.Email);
            if (student == null)
            {
                return Ok();
            }
            bool isValidRefreshToken = BCrypt.Net.BCrypt.Verify(request.RefreshToken, student.RefreshTokenHash);
            if (!isValidRefreshToken)
            {
                return Ok();
            }
                student.RefreshTonkenRevokedAt = DateTime.UtcNow;
            return Ok(new { message = "Logged out successfully." });
        }
    }

            
              
}
