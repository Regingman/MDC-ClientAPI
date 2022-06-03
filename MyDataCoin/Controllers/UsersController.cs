using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyDataCoin.Entities;
using MyDataCoin.Interfaces;
using MyDataCoin.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace NuzaiCore.Controllers.v2
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class usersController : ControllerBase
    {
        private readonly ILogger<usersController> _logger;
        private readonly IUser _userService;

        public usersController(ILogger<usersController> logger, IUser userService)
        {
            _userService = userService;
            _logger = logger;
        }


        /// <summary>
        /// User authentication, pass "meta", "google", or "apple" in SocialNetwork paramter,
        /// in case if it's mapping proccess, put UserId parameter
        /// </summary>
        /// <response code="200">Returns JWT token</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(AuthenticateResponse))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Type = typeof(GeneralResponse))]
        [AllowAnonymous]
        [HttpPost]
        [Route("auth")]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateRequest model)
        {
            AuthenticateResponse response = await _userService.Authenticate(model);

            if (response.Code == 400)
                return BadRequest(response.Message);
            else
                return Ok(response);
        }



        /// <summary>
        /// Refresh token
        /// </summary>
        /// <response code="200">Returns JWT token</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(AuthenticateResponse))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Type = typeof(GeneralResponse))]
        [AllowAnonymous]
        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh([FromBody] Tokens tokens)
        {
            RefreshResponse response =  _userService.Refresh(tokens);

            if (response.Code == 400)
                return BadRequest(response.Message);
            else if (response.Code == 401)
                return Unauthorized();
            else
                return Ok(response);
        }



        /// <summary>
        /// Map accounts
        /// </summary>
        /// <response code="200">Returns Ok</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(AuthenticateResponse))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Type = typeof(GeneralResponse))]
        [Authorize]
        [HttpPost]
        [Route("map/{userid}")]
        public async Task<IActionResult> MapAccounts(string userid, [FromBody] AuthenticateRequest model)
        {
            GeneralResponse response = await _userService.Mapping(userid, model);

            if (response.Code == 400)
                return BadRequest(response.Message);
            else if (response.Code == 401)
                return Unauthorized();
            else
                return Ok(response);
        }



        /// <summary>
        /// Upload user photo, accepts only base64 string only
        /// </summary>
        /// <response code="200">Returns JWT token</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(AuthenticateResponse))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Type = typeof(GeneralResponse))]
        [Authorize]
        [HttpPost]
        [Route("upload_image")]
        public async Task<IActionResult> UploadPhoto([FromBody] Uploadrequest model)
        {
            GeneralResponse response = await _userService.Upload(model);

            if (response.Code == 400)
                return BadRequest(response.Message);
            else
                return Ok(response);
        }




        /// <summary>
        /// Edit user
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GeneralResponse))]
        [SwaggerResponse(421, Type = typeof(GeneralResponse))]
        [HttpPut("edit/{userid}")]
        public async Task<IActionResult> Edit(string userid, [FromBody] EditRequest user)
        {
            var result = await _userService.EditUser(userid, user);
            if (result.Code == 421) return StatusCode(421, new GeneralResponse(421, "User Not Found"));
            else return Ok(new GeneralResponse(200, "Success"));
        }




        /// <summary>
        /// Get Balance and info about Unread notifications
        /// </summary>
        /// <response code="200">Returns Ok</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(MainInfo))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Type = typeof(GeneralResponse))]
        [Authorize]
        [HttpGet]
        [Route("main_info/{userid}")]
        public async Task<IActionResult> GetInfoForMain(string userid)
        {
            MainInfo response = await _userService.GetInfoForMain(userid);
            return Ok(response);
        }





        /// <summary>
        /// Register new user
        /// </summary>
        /// <response code="201">Returns Created</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="430">Returns Code Doesn't Send</response>
        /// <response code="431">Returns User Not Verified</response>
        /// <response code="500">Returns Internal Server Error</response>
        //[AllowAnonymous]
        //[HttpPost("register")]
        //[SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(AuthenticateResponse))]
        //[SwaggerResponse(409, Type = typeof(GeneralResponse))]
        //[SwaggerResponse(430, Type = typeof(GeneralResponse))]
        //[SwaggerResponse(431, Type = typeof(GeneralResponse))]
        //public async Task<IActionResult> Registration(RegistrationRequest model)
        //{
        //    AuthenticateResponse response = await _userService.Registration(model);
        //    if (response.Code == 430) return StatusCode(430, new GeneralResponse(response.Code, response.Message));
        //    if (response.Code == 431) return StatusCode(431, new GeneralResponse(response.Code, response.Message));
        //    if (response.Code == 409) return StatusCode(409, new GeneralResponse(response.Code, response.Message));
        //    else return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
        //}


        /// <summary>
        /// Register new user via socail buttons
        /// </summary>
        /// <response code="201">Returns Created</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="500">Returns Internal Server Error</response>
        //[AllowAnonymous]
        //[HttpPost("social_register")]
        //[SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(AuthenticateResponse))]
        //public async Task<IActionResult> SocialRegistration(SocialRegistrarionRequest model)
        //{
        //    AuthenticateResponse response = await _userService.SocialRegistration(model);
        //    return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
        //}


        /// <summary>
        /// Get user by id
        /// </summary>
        /// <response code="200">Returns User</response>
        /// <response code="204">Returns No Content</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="403">Returns Forbidden</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response> 
        //[Authorize]
        //[HttpGet("getbyid/{id}")]
        //[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(User))]
        //[SwaggerResponse(421, Type = typeof(GeneralResponse))]
        //public async Task<IActionResult> GetById(string email)
        //{
        //    var user = await _userService.GetByEmail(email);
        //    if (user == null) return StatusCode(421, new GeneralResponse(421, "User Not Found"));
        //    else return Ok(user);
        //}




        /// <summary>
        /// Send verification code to user
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Email cannot be empty</response>
        /// <response code="409">Returns User Exists</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="420">Returns Invalid Email</response>
        /// <response code="500">Returns Internal Server Error</response>
        //[HttpGet("sendcode/{email}")]
        //[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GeneralResponse))]
        //[SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(GeneralResponse))]
        //[SwaggerResponse(409, Type = typeof(GeneralResponse))]
        //[SwaggerResponse(420, Type = typeof(GeneralResponse))]
        //[SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(GeneralResponse))]
        //public async Task<IActionResult> SendCode(string email)
        //{
        //    bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

        //    if (!isEmail) return StatusCode(420, new GeneralResponse(420,"Invalid Email"));
        //    else
        //    {
        //        if (email == null) return StatusCode(400, new GeneralResponse(400, "Email cannot be empty"));
        //        else
        //        {
        //            GeneralResponse result = await _userService.SendCode(email);
        //            if (result.Code == 200) return StatusCode(200, new GeneralResponse(200, result.Message));
        //            else if (result.Code == 409) return StatusCode(409, new GeneralResponse(409, result.Message));
        //            else return StatusCode(500, new GeneralResponse(500, result.Message));
        //        }
        //    }
        //}


        /// <summary>
        /// Verify user by code, sent to an email
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="420">Returns Wrong Code</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        //[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GeneralResponse))]
        //[SwaggerResponse(420, Type = typeof(GeneralResponse))]
        //[SwaggerResponse(421, Type = typeof(GeneralResponse))]
        //[HttpGet("verify/{email}/code/{code}")]
        //public async Task<IActionResult> Verify(string email, string code)
        //{
        //    GeneralResponse result = await _userService.VerifyUser(email, code);
        //    if (result.Code == 400) return BadRequest(new { message = result.Message });
        //    else if (result.Code == 420) return StatusCode(420, result.Message);
        //    else if (result.Code == 421) return StatusCode(421, result.Message);
        //    else return StatusCode(200, new GeneralResponse(420, result.Message));
        //}


        /// <summary>
        /// Send new password to user
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="409">Returns User Doesn't Exist</response>
        /// <response code="420">Returns Invalid Email</response>
        /// <response code="500">Returns Internal Server Error</response>
        //[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GeneralResponse))]
        //[SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(GeneralResponse))]
        //[HttpGet("resetpassword/{email}")]
        //public async Task<IActionResult> ResetPassword(string email)
        //{
        //    _logger.LogInformation($"Request received with email {email}");
        //    bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

        //    if (!isEmail) return StatusCode(420, "Invalid Email");
        //    else
        //    {
        //        GeneralResponse result = await _userService.ResetPassword(email);
        //        _logger.LogInformation($"Result is {result}");
        //        if (result.Code == 200) return StatusCode(200, new GeneralResponse(200, "Success"));
        //        else if (result.Code == 409) return StatusCode(409, new GeneralResponse(409, "User Doesn't Exist"));
        //        else return BadRequest(result.Message);
        //    }
        //}
    }
}
