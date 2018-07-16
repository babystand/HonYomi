using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HonYomi.ApiControllers
{
    [ApiController]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        [Route("/api/account/login")]
        public async Task<object> Login([FromBody] LoginDto model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (result.Succeeded)
            {
                var user = await userManager.Users.SingleOrDefaultAsync(r => r.UserName == model.Username);
                return GenerateJwtToken(model.Username, user);
            }
            throw new ApplicationException("Bad login");
        }
        
        //todo: disable for production
        [HttpPost]
        [Authorize]
        [Route("/api/account/register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new IdentityUser
                       {
                           UserName = model.Username
                       };
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok();
            }
            
            throw new ApplicationException("UNKNOWN_ERROR");
        }
        private string GenerateJwtToken(string username, IdentityUser user)
        {
            var claims = new List<Claim>
                         {
                             new Claim(JwtRegisteredClaimNames.Sub, username),
                             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                             new Claim(ClaimTypes.NameIdentifier, user.Id)
                         };

            var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RuntimeConstants.JwtKey));
            var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(30));

            var token = new JwtSecurityToken(
                                             null,
                                             null,
                                             claims,
                                             expires: expires,
                                             signingCredentials: creds
                                            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public class LoginDto
        {
            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }

        }
        
        public class RegisterDto
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength = 8)]
            public string Password { get; set; }
        }
    }
}