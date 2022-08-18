using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.BLL.Interfaces;
using Talabat.DAL.Entities.Identity;

namespace Talabat.APIs.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> userManager ,
            SignInManager<AppUser> signInManager ,
            ITokenService tokenService,
            IMapper mapper
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Unauthorized(new ApiResponse(401));
            var result =await signInManager.CheckPasswordSignInAsync(user, loginDto.Password , false);
            if(!result.Succeeded) return Unauthorized(new ApiResponse(401));
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await tokenService.CreateToken(user , userManager)
            });

        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (CheckEmailExist(registerDto.Email).Result.Value)
                return BadRequest(new ApiValidationErrorResponse() { Errors = new[] {"This email is already Exists!"}});
            var user = new AppUser()
            {
                Email = registerDto.Email,
                UserName = registerDto.Email.Split("@")[0],
                DisplayName = registerDto.DisplayName,
                PhoneNumber = registerDto.PhoneNumber,
                Address = new Address()
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    City = registerDto.City,
                    Country = registerDto.Country,
                    Street = registerDto.Street
                }
            };
            var result = await userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await tokenService.CreateToken(user, userManager)
            });



        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user =await userManager.FindByEmailAsync(email);
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token =await tokenService.CreateToken(user , userManager)               
            });
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user =await userManager.FindUserAndAddressByEmailAsync(User);

            return Ok(mapper.Map<Address, AddressDto>(user.Address));
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto NewAddress)
        {
            var user = await userManager.FindUserAndAddressByEmailAsync(User);

            user.Address = mapper.Map<AddressDto, Address>(NewAddress);

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(new ApiValidationErrorResponse() { Errors = new[] {"An Error Occured With Updating User Address"} });
            return Ok(NewAddress);
        }

        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExist([FromQuery]string email)
        {
            return await userManager.FindByEmailAsync(email) != null; 
        }
    }
}
