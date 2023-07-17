using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToDoListApp.BLL.Exceptions;
using ToDoListApp.BLL.Models.Dto;
using ToDoListApp.BLL.Services.Interfaces;
using ToDoListApp.Contracts.Requests;
using ToDoListApp.Contracts.Responses;
using ToDoListApp.DAL.Entity.Identity;
using ToDoListApp.Enums;

namespace ToDoListApp.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<UserRoles> roleManager;
        private readonly IConfiguration _configuration;
        public AuthService(UserManager<User> userManager, RoleManager<UserRoles> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<UserDto> Registeration(RegistrationRequestModel model, Roles roles)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);

            if (userExists != null)
            {
                throw new UserNameAlreadyExistsException();
            }

            userExists = await userManager.FindByEmailAsync(model.Email);

            if (userExists?.Email == model.Email)
            {
                throw new EmailAlreadyException();
            }               

            User user = new()
            {
                Email = model.Email,
                UserName = model.Username
            };

            var createUserResult = await userManager.CreateAsync(user, model.Password);

            if (!createUserResult.Succeeded)
            {
                throw new UserNotCreatedException();
            }

            if (!await roleManager.RoleExistsAsync(Roles.User.ToString()))
            {
                await roleManager.CreateAsync(new UserRoles { Name = roles.ToString() });
            }

             await userManager.AddToRoleAsync(user, Roles.User.ToString());

            return new UserDto
            {
                Email = model.Email,
                UserName = model.Username,
            };
        }

        public async Task<TokenResponseModel> Login(LoginRequestModel model)
        {
            TokenResponseModel _TokenViewModel = new();
            var user = await userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                //_TokenViewModel.StatusCode = 0;
                //_TokenViewModel.StatusMessage = "Invalid username";
                //return _TokenViewModel;
                throw new UserNotFoundException();
            }

            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                //_TokenViewModel.StatusCode = 0;
                //_TokenViewModel.StatusMessage = "Invalid password";
                //return _TokenViewModel;
                throw new InvalidPasswordException();
            }

            var userRoles = await userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            _TokenViewModel.AccessToken = GenerateToken(authClaims);
            _TokenViewModel.RefreshToken = GenerateRefreshToken();
            //_TokenViewModel.StatusCode = 1;
            //_TokenViewModel.StatusMessage = "Success";

            var _RefreshTokenValidityInDays = Convert.ToInt64(_configuration["JWTKey:RefreshTokenValidityInDays"]);
            user.RefreshToken = _TokenViewModel.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_RefreshTokenValidityInDays);


            await userManager.UpdateAsync(user);

            return _TokenViewModel;
        }

        public async Task<TokenResponseModel> GetRefreshToken(GetRefreshTokenRequestModel model)
        {
            TokenResponseModel _TokenViewModel = new();
            var principal = GetPrincipalFromExpiredToken(model.AccessToken);
            string username = principal.Identity.Name;
            var user = await userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                //_TokenViewModel.StatusCode = 0;
                //_TokenViewModel.StatusMessage = "Invalid access token or refresh token";
                throw new AccessTokenInvalidException();
                //return _TokenViewModel;
            }

            var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var newAccessToken = GenerateToken(authClaims);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await userManager.UpdateAsync(user);

            //_TokenViewModel.StatusCode = 1;
            //_TokenViewModel.StatusMessage = "Success";
            _TokenViewModel.AccessToken = newAccessToken;
            _TokenViewModel.RefreshToken = newRefreshToken;

            return _TokenViewModel;
        }


        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));
            var _TokenExpiryTimeInHour = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInHour"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWTKey:ValidIssuer"],
                Audience = _configuration["JWTKey:ValidAudience"],
                Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
                //Expires = DateTime.Now.AddMinutes(10),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public async Task DeleteToken(string username)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                throw new InvalidUserNameException();
            }

            user.RefreshToken = null;
            await userManager.UpdateAsync(user);
        }
        public async Task DeleteTokenAll()
        {
            var users = userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await userManager.UpdateAsync(user);
            }
        }
    }
}
