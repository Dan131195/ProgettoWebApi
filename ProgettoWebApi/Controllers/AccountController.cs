using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProgettoWebApi.Models.Auth;
using ProgettoWebApi.Settings;
using ProgettoWebApi.DTOs.Account;

namespace ProgettoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly Jwt _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(IOptions<Jwt> jwtOptions, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _jwtSettings = jwtOptions.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var newUser = new ApplicationUser()
            {
                Email = registerRequestDto.Email, // Imposta l'email dell'utente
                UserName = registerRequestDto.Email, // Imposta il nome utente uguale all'email
                Nome = registerRequestDto.FirstName, // Imposta il nome dell'utente
                Cognome = registerRequestDto.LastName, // Imposta il cognome dell'utente
            };

            // Crea l'utente nel database con la password specificata
            var result = await _userManager.CreateAsync(newUser, registerRequestDto.Password);

            // Se la creazione dell'utente non ha avuto successo, reindirizza alla pagina dei prodotti
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            // Trova l'utente appena creato in base alla sua email
            var user = await _userManager.FindByEmailAsync(newUser.Email);

            // Aggiunge l'utente al ruolo "User"
            await _userManager.AddToRoleAsync(newUser, "SuperAdmin");

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Email);

            await _signInManager.PasswordSignInAsync(user, loginRequestDto.Password, false, false);

            var roles = await _signInManager.UserManager.GetRolesAsync(user);

            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Name, $"{user.Nome} {user.Cognome}"));
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddMinutes(_jwtSettings.ExpiresInMinutes);

            var token = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, claims, expires: expiry, signingCredentials: creds);

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new TokenResponse()
            {
                Token = tokenString,
                Expires = expiry
            });
        }
    }
}
