using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RedMango_API.Data;
using RedMango_API.Models;
using RedMango_API.Models.Dto;
using RedMango_API.Services;
using RedMango_API.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace RedMango_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ApiResponse _response;
        private string secretKey;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
       
        public AuthController(ApplicationDbContext db, IConfiguration configuration, 
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _response = new ApiResponse();
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _roleManager = roleManager;
            _userManager = userManager;
            
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto model)
        {
            ApplicationUser userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == model.UserName);
            

            var isValid = await _userManager.CheckPasswordAsync(userFromDb, model.Password);

            if (!isValid)
            {
                _response.Result = new LoginResponseDto();
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Username or Password Is Incorrect");
                _response.IsSuccess= false;
                return BadRequest(_response);
            }

            //generate JWT token
            var roles = await _userManager.GetRolesAsync(userFromDb);
            var symmetricKey = Encoding.ASCII.GetBytes(secretKey);
            JwtSecurityTokenHandler tokenHandler= new JwtSecurityTokenHandler();

            SecurityTokenDescriptor securityTokenDescriptor = new ()
            { 
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Fullname", userFromDb.Name),
                    new Claim("Id", userFromDb.Id.ToString()),
                    new Claim(ClaimTypes.Email, userFromDb.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    
                }),
                Expires= DateTime.UtcNow.AddMinutes(10),

                SigningCredentials= new SigningCredentials(new SymmetricSecurityKey(symmetricKey),
                SecurityAlgorithms.HmacSha256Signature),
             
            };

            var stoken = tokenHandler.CreateToken(securityTokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);



            LoginResponseDto loginResponseDto = new()
            {
                Email = userFromDb.UserName,
                Token = token
            };

            if (loginResponseDto.Email == null || string.IsNullOrEmpty(loginResponseDto.Token))
            {
                _response.Result = new LoginResponseDto();
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Username or Password Is Incorrect");
                _response.IsSuccess = false;
                return BadRequest(_response);

            }

            _response.Result = loginResponseDto;
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        {
            
                ApplicationUser userFromDb = _db.ApplicationUsers
                .FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());

                if (userFromDb != null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Username Already Exist");
                    return BadRequest(_response);
                }



                ApplicationUser newUser = new()
                {
                    UserName = model.UserName,
                    Email = model.UserName,
                    NormalizedEmail = model.UserName.ToUpper(),
                    Name = model.Name,
                };

            try
            {

                var result = await _userManager.CreateAsync(newUser, model.Password);

                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                    {
                        //create roles in database
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                    }

                    if (model.Role.ToLower() == SD.Role_Admin)
                    {
                        await _userManager.AddToRoleAsync(newUser, SD.Role_Admin);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);
                    }

                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    return Ok(_response);

                }

                

            }
            catch (Exception ex)
            {
               return BadRequest(ex.Message);

            }

            _response.IsSuccess = false;
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.ErrorMessages.Add("Registration Was Not Successful");
            return BadRequest(_response);


        }
    }
}
