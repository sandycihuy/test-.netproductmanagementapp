using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductManagementApp.Models;
using ProductManagementApp.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;

namespace ProductManagementApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IEmailService emailService,
            IWebHostEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
            _env = env;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                
                var callbackUrl = Url.Action("ConfirmEmail", "Auth", 
                    new { userId = user.Id, token = encodedToken }, 
                    protocol: Request.Scheme);
                var emailSubject = "Konfirmasi Email Anda";
                var emailMessage = $"Silakan konfirmasi email Anda dengan <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>mengklik di sini</a>.";
                
                await _emailService.SendEmailAsync(user.Email, emailSubject, emailMessage);

                return Ok(new { Message = "Registrasi berhasil. Silakan periksa email Anda untuk konfirmasi." });
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("User ID dan token tidak valid");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User tidak ditemukan");
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                return Ok("Email berhasil dikonfirmasi. Silakan login.");
            }

            return BadRequest("Gagal mengonfirmasi email");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if (user == null)
            {
                return Unauthorized("Email atau password salah");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Unauthorized("Email belum dikonfirmasi. Silakan periksa email Anda.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, 
                model.Password, 
                isPersistent: false, 
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(user, roles);
                return Ok(new { Token = token });
            }

            return Unauthorized("Email atau password salah");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "Logout successful" });
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Add roles to claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Nama lengkap minimal {2} karakter.", MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password minimal {2} karakter.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password dan konfirmasi password tidak cocok.")]
        public string ConfirmPassword { get; set; }
    }
}