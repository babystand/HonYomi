﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLib;
using Hangfire.MemoryStorage.Database;
using HonYomi.Database;
using HonYomi.Exposed;
using Microsoft.AspNetCore.Antiforgery.Internal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace HonYomi.ApiControllers
{
    [ApiController]
    public class MediaController : Controller
    {
        [HttpGet, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme),
         Route("/api/books/settrack/{trackId}")]
        public IActionResult SetTrack(Guid trackId)
        {
            try
            {
                DataAccess.SetCurrentTrack(User.Identity.Name, trackId);
                return Json(DataAccess.GetUserFileProgress(User.Identity.Name, trackId));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

      
        [HttpGet]
        [Route("/api/tracks/stream/{location}")]
        //accepts byte range headers
        public IActionResult GetAudioFile(string location)
        {
            ( Guid id, string token ) = SplitTrackAddress(location);
            if (!Validate(token)) return BadRequest("Invalid JWT");
            string path, mimeType;
            using (var db = new HonyomiContext())
            {
                var file = db.Files.FirstOrDefault(x => x.IndexedFileId == id);
                if (file == null)
                {
                    return NotFound();
                }

                path     = file.FilePath;
                mimeType = file.MimeType;
            }

            return File(System.IO.File.OpenRead(path), mimeType, true);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("/api/tracks/progress/get/{trackId}")]
        public IActionResult GetTrackProgress(Guid trackId)
        {
            try
            {
                return Json(DataAccess.GetUserFileProgress(User.Identity.Name, trackId));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("/api/tracks/progress/set/{trackId}/{seconds}")]
        public IActionResult SetTrackProgress(Guid trackId, double seconds)
        {
            try
            {
               DataAccess.SetTrackProgress(User.Identity.Name, trackId, seconds);
                    return Json(true);
                
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet]
        [Route("/api/getlongfile")]
        public FileStreamResult GetLongFile()
        {
            const string path = "../HonYomi.Tests/long.mp3";
            return File(System.IO.File.OpenRead(path), "audio/mpeg", true);
        }

        private static (Guid id, string jwt) SplitTrackAddress(string location)
        {
            try
            {
                string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(location));
                string[] parts = decoded.Split(':');
                return ( Guid.Parse(parts[0]), parts[1] );
            }
            catch (Exception)
            {
                return ( Guid.Empty, "" );
            }
        }
        
        private static bool Validate(string jwt)
        {
            var validationParameters = new TokenValidationParameters
            {
                // Clock skew compensates for server time drift.
                // We recommend 5 minutes or less:
                ClockSkew = TimeSpan.FromMinutes(5),
                // Specify the key used to sign the token:
                IssuerSigningKey    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RuntimeConstants.JwtKey)),
                RequireSignedTokens = true,
                // Ensure the token hasn't expired:
                RequireExpirationTime = true,
                ValidateLifetime      = true,
                // Ensure the token audience matches our audience value (default true):
                ValidateAudience = true,
                ValidAudience    = RuntimeConstants.JwtIssuer,
                // Ensure the token was issued by a trusted authorization server (default true):
                ValidateIssuer = true,
                ValidIssuer    = RuntimeConstants.JwtIssuer
            };

            try
            {
                var claimsPrincipal = new JwtSecurityTokenHandler()
                    .ValidateToken(jwt, validationParameters, out var rawValidatedToken);

                return true;
                // Or, you can return the ClaimsPrincipal
                // (which has the JWT properties automatically mapped to .NET claims)
            }
            catch (SecurityTokenValidationException stvex)
            {
                return false;
            }
            catch (ArgumentException argex)
            {
                return false;
            }
        }
    }
}