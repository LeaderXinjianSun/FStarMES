using FStarMES.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FStarMES.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public UserDto Login(LoginDto dto)
        {
            var jwtToken = GetToken(dto.UserName);
            return new() { Name = dto.UserName,Token = jwtToken};
        }
        [HttpGet]
        public UserDto GetUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var name = User.Claims.First(x => x.Type == ClaimTypes.Name).Value;
                var jwtToken = GetToken(name);
                return new() { Name = name,Token = jwtToken};
            }
            else
            {
                return new() { Name = null,Token = null};
            }
        }
        public string GetToken(string name)
        {
            var claims = new Claim[] {
                new Claim(ClaimTypes.Name,name),
                new Claim(ClaimTypes.Role,"Admin"),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("123456789012345678901234567890123456789"));
            var expires = DateTime.Now.AddDays(30);
            var token = new JwtSecurityToken(
                issuer: "guetSever",
                audience: "guetClient",
                claims: claims,
                notBefore: DateTime.Now,
                expires: expires,
                signingCredentials: new SigningCredentials(key,SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
