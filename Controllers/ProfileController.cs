using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductManagementApp.Models;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductManagementApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _env = env;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(new ProfileViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                ProfilePictureUrl = user.ProfilePicture
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

        
            user.FullName = model.FullName ?? user.FullName;
            user.Email = model.Email ?? user.Email;

            if (model.ProfilePicture != null)
            {
                var uploadResult = await SaveProfilePicture(model.ProfilePicture);
                if (!uploadResult.Success)
                {
                    return BadRequest(new { message = uploadResult.ErrorMessage });
                }
                user.ProfilePicture = uploadResult.FilePath;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
           
            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                user.EmailConfirmed = false;
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
               
                _logger.LogInformation($"Email confirmation token for {user.Email}: {token}");
            }

            return Ok(new { message = "Profile updated successfully" });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "Password changed successfully" });
        }

        private async Task<(bool Success, string FilePath, string ErrorMessage)> SaveProfilePicture(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return (false, null, "No file uploaded");
                }

    
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    return (false, null, "Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");
                }

              
                if (file.Length > 5 * 1024 * 1024)
                {
                    return (false, null, "File size cannot exceed 5MB");
                }

               
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "profile-pictures");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

              
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

             
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                
                var relativePath = $"/uploads/profile-pictures/{uniqueFileName}";
                return (true, relativePath, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving profile picture");
                return (false, null, "An error occurred while saving the file");
            }
        }
    }

    public class ProfileViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ProfilePictureUrl { get; set; }
    }

    public class UpdateProfileModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public IFormFile ProfilePicture { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
