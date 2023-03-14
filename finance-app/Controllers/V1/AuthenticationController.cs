using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using finance_app.Types.Services.V1.Interfaces;
using finance_app.Types.DataContracts.V1.Responses;
using finance_app.Types.DataContracts.V1.Dtos;
using AutoMapper;
using finance_app.Types.DataContracts.V1.Requests.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace finance_app.Controllers.V1
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationService _authencationService;
        
        private readonly IMapper _mapper;

        public AuthenticationController(ILogger<AuthenticationController> logger,
                                  IMapper mapper, IAuthenticationService authenticationService
                                  )
        {
            _logger = logger;
            _mapper = mapper;
            _authencationService = authenticationService;
        }

        /// <summary>
        /// Logs a user into the system
        /// </summary>
        /// <param name="loginRequest">A Login request</param>
        /// <remarks> 
        /// </remarks>
        /// <returns>A Logged in user</returns>
        [HttpPost]
        [Route("api/Login")]
        [UserAuthorizationFilter]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {

            var ret = await _authencationService.Login(loginRequest, HttpContext);

            // TODO: Get Roles, Set Claims

            //var appUser = _financeAppDbContext.ApplicationUsers
            //    .Include(x => x.ApplicationUserRoles)
            //    .Where(x => x.AuthenticationUserId == authenticationUser.Id)
            //    .FirstOrDefault();

            //if (appUser == null) {
            //    // TODO: Fix Response to avoid givving information to a user;
            //    var errorMessage = new ErrorResponseMessage(
            //            new LogginInActionMessage(loginRequest.Username),
            //            new ResourceWithPropertyMessage(authenticationUser, "Username", loginRequest.Username),
            //            new UnauthorizedToAccessResourceReason());
            //    return new ApiResponse<AuthenticationUserDto>(ApiResponseCodesEnum.Unauthorized, errorMessage);
            //}


            //var claimsIdentity = new ClaimsIdentity(appUser.GetClaims(authenticationUser), CookieAuthenticationDefaults.AuthenticationScheme);

            //await _microsfotAuthService.SignInAsync(httpContext, null, new ClaimsPrincipal(claimsIdentity), null);

            return StatusCode(_mapper.Map<int>(ret?.ResponseCode), ret);
        }

        /// <summary>
        /// Creates a user
        /// </summary>
        /// <param name="createUserRequest">A Create User request</param>
        /// <remarks> 
        /// </remarks>
        /// <returns>A Logged in user</returns>
        [HttpPost]
        [Route("api/Create")]
        [UserAuthorizationFilter]
        public async Task<IActionResult> Create(CreateAuthenticationUserRequest createUserRequest) {
            var ret = await _authencationService.CreateAuthUser(createUserRequest);
            
            return StatusCode(_mapper.Map<int>(ret?.ResponseCode), ret);

        }



    }
}
