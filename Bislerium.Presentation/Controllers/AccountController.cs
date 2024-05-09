using Bislerium.Application.IServices;
using Bislerium.Domain.Entity.Users;
using Bislerium.Domain.Statics;
using Bislerium.Domain.ViewModels;
using Bislerium.Infrastructure.Data;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Week21.Domain;

namespace Bislerium.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IFirebaseService _firebase;

        public AccountController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config,
            SignInManager<ApplicationUser> signInManager,
            IEmailService email,
            IFirebaseService firebase)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _signInManager = signInManager;
            _emailService = email;
            _firebase = firebase;
        }

        [HttpPost("registerUser")]
        public async Task<IActionResult> RegisterUser(Register model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser { Name = model.Name, UserName = model.Email, Email = model.Email, ProfileUrl = "dummyProfile.jpg", CoverUrl = "dummyCover.jpg" };

            // Check if the specified role exists
            var roleExists = await _roleManager.RoleExistsAsync(model.Role);
            if (!roleExists)
            {
                // If the role doesn't exist, return error
                return BadRequest("Invalid role specified.");
            }

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Assign the specified role to the user
                await _userManager.AddToRoleAsync(user, model.Role);
                return Ok("User registered successfully.");
            }
            return BadRequest(result.Errors);
        }

        [HttpGet("GetUser/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var users = await _userManager.FindByIdAsync(id);
            return Ok(users);
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users.ToListAsync();
            List<UserVM> usersWithRoles = new List<UserVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userWithRole = new UserVM
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Address = user.Address,
                    Role = roles.FirstOrDefault()
                };
                usersWithRoles.Add(userWithRole);
            }

            return Ok(usersWithRoles);
        }


        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok("Succeeded");
            }
            return BadRequest(result.Errors);
        }

        [HttpPut("EditProfile")]
        public async Task<IActionResult> EditProfile(ApplicationUser appUser)
        {
            var user = await _userManager.FindByIdAsync(appUser.Id);
            if (user == null)
            {
                return NotFound();
            }
            user.PhoneNumber = appUser.PhoneNumber;
            user.Name = appUser.Name;
            if (!string.IsNullOrEmpty(appUser.ProfileUrl))
            {
                user.ProfileUrl = appUser.ProfileUrl;
            }
            if (!string.IsNullOrEmpty(appUser.CoverUrl))
            {
                user.CoverUrl = appUser.CoverUrl;
            }
            user.Address = appUser?.Address;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(user);
            }
            return BadRequest();
        }

        [HttpPut("UpdatePassword/{userId}")]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model, string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
            if (changePasswordResult.Succeeded)
            {
                return Ok(model);
            }

            return BadRequest(changePasswordResult.Errors);
        }

        [HttpPost]
        [Route("LoginUser")]
        public async Task<LoginResponse> Login([FromBody] LoginUser loginUser)
        {
            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {

                var getUser = await _userManager.FindByEmailAsync(loginUser.Email);
                var getUserRole = await _userManager.GetRolesAsync(getUser);
                var userSession = new UserSession(getUser.Id, getUser.Name, getUser.Email, getUserRole.First());
                string token = GenerateToken(userSession);

                return new LoginResponse(true, token!, "Login completed");

            }
            else
            {
                return new LoginResponse(false, null!, "Login not completed");
            }
        }

        private string GenerateToken(UserSession user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost, Route("ResetEmail/{request}")]
        public async Task<IActionResult> ResetPasswordEmail(string request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request);
                if (user == null)
                {
                    return Ok("Password reset email sent successfully.");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var frontendUrl = "https://localhost:7105";
                var resetLink = $"{frontendUrl}/Account/ResetPasswordConfirm?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

                await _emailService.SendMail(new EmailMessage()
                {
                    To = user.Email,
                    Subject = "Reset Password",
                    Body = $"Please reset your password by clicking here: <a href='{resetLink}'>link</a>"
                });

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost, Route("GetToken/{email}")]
        public async Task<IActionResult> ResetPassword(TokenVM tokenVM)
        {
            var user = await _userManager.FindByEmailAsync(tokenVM.Email);
            if (user == null)
            {
                return BadRequest("Sorry bad request");
            }

            var isValidToken = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", tokenVM.Token);
            if (!isValidToken)
            {
                return BadRequest("Sorry bad request");
            }

            return Ok(new { Email = tokenVM.Email, Token = tokenVM.Token });

        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            try
            {
                if (model.NewPassword == model.CNewPassword)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        return BadRequest("User Not found");
                    }

                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return Ok("Password reset successful.");
                    }
                }
                return BadRequest("Failed to reset password.");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("SaveFCMToken")]
        public async Task<IActionResult> SaveFCMToken(CreateToken payload)
        {
            var result = await _firebase.CreateNewToken(payload);
            return Ok(result);
        }

    }
}
