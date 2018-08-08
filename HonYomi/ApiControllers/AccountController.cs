using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DataLib;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HonYomi.ApiControllers
{
    [ApiController]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser>   userManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager   = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        [Route("/api/auth/login")]
        public IActionResult Login([FromBody] UserCreds model)
        {
            var result = signInManager.PasswordSignInAsync(model.Username, model.Password, false, false).Result;
            if (result.Succeeded)
            {
                var user = userManager.Users.SingleOrDefaultAsync(r => r.UserName == model.Username).Result;
                return Json(GenerateJwtToken(model.Username, user));
            }

            return BadRequest();
        }

        //todo: disable for production
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("/api/auth/register")]
        public IActionResult Register([FromBody] UserCreds model)
        {
            var user = new IdentityUser
            {
                UserName = model.Username
            };
            var result = userManager.CreateAsync(user, model.Password).Result;

            if (result.Succeeded)
            {
                return Json(true);
            }

            return BadRequest();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("/api/auth/changeusername/{username}")]
        public IActionResult ChangeUsername(string username)
        {
            IdentityUser user   = userManager.FindByIdAsync(User.Identity.Name).Result;
            var          result = userManager.SetUserNameAsync(user, username).Result;
            if (result.Succeeded)
            {
                return Json(true);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("/api/auth/changepassword")]
        public IActionResult ChangePassword([FromBody] UserCreds model)
        {
            IdentityUser user = userManager.FindByIdAsync(User.Identity.Name).Result;
            try
            {
                var result = userManager.ChangePasswordAsync(user, model.Password, model.NewPassword).Result;
                if (result.Succeeded)
                {
                    return Json(true);
                }

                return BadRequest(result.Errors);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme), Route("/api/auth/refresh")]
        public IActionResult Refresh()
        {
            IdentityUser user = userManager.FindByIdAsync(User.Identity.Name).Result;
            return Json(GenerateJwtToken(User.Identity.Name, user));
        }

        private TokenObject GenerateJwtToken(string username, IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user.Id)
            };

            var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RuntimeConstants.JwtKey));
            var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(30));

            var token = new JwtSecurityToken(
                RuntimeConstants.JwtIssuer,
                RuntimeConstants.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new TokenObject(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public class TokenObject
        {
            public string Token { get; set; }

            public TokenObject(string tok)
            {
                Token = tok;
            }
        }

        public class UserCreds
        {
            public string Username { get; set; }

            public string Password { get; set; }

            [JsonProperty(PropertyName = "newPassword")]
            public string NewPassword { get; set; }
        }
    }
}