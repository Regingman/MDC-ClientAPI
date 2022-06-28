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
        [AllowAnonymous]
        [HttpPost]
        [Route("auth")]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateRequest model)
        {
            _logger.LogInformation($"Auth request with {model.Email} and {model.SocialId}, {model.SocialNetwork} and {model.NickName}");
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
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Tokens))]
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
        /// <response code="436">Returns Already Mapped</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GeneralResponse))]
        [Authorize]
        [HttpPost]
        [Route("map/{userid}")]
        public async Task<IActionResult> MapAccounts(string userid, [FromBody] MappingRequest model)
        {
            _logger.LogInformation($"Request from Mapping with data {model.Email}, {model.SocialId} and {model.SocialNetwork}");

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
        [HttpPut("edit/{userid}")]
        public async Task<IActionResult> Edit(string userid, [FromBody] EditRequest user)
        {
            var result = await _userService.EditUser(userid, user);
            if (result.Code == 421) return StatusCode(421, new GeneralResponse(421, "User Not Found"));
            else return Ok(new GeneralResponse(200, "Success"));
        }




        /// <summary>
        /// Get statistics of reffered people and total rewards for inviting
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="500">Returns Internal Server Error</response>
        //[Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(StatisticsOfRefferedPeopleModel))]
        [HttpGet("promo/get_statistics/{userid}")]
        public async Task<StatisticsOfRefferedPeopleModel> GetRefferedPeople(string userid)
        {
            return await _userService.GetRefferedPeople(userid);
        }


        /// <summary>
        /// Get user for main page by id
        /// </summary>
        /// <response code="200">Returns UserForMainPage Model</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="436">Returns Already Mapped</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(UserForMainPage))]
        [Authorize]
        [HttpGet]
        [Route("getbyid/{userid}")]
        public async Task<IActionResult> GetById(string userid)
        {
            var response = await _userService.GetById(userid);
            return Ok(response);
        }


        /// <summary>
        /// Get Privacy Policy page
        /// </summary>
        /// <response code="200">Returns Privacy Policy page</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="436">Returns Already Mapped</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string))]
        [AllowAnonymous]
        [HttpGet]
        [Route("privacy")]
        public string GetPrivacy()
        {
            return _userService.GetPrivacy();
        }

        /// <summary>
        /// Get Privacy Terms Of Services page
        /// </summary>
        /// <response code="200">Returns UserForMainPage Model</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="436">Returns Already Mapped</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string))]
        [AllowAnonymous]
        [HttpGet]
        [Route("terms")]
        public string GetTerms()
        {
            return _userService.GetTerms();
        }
    }
}
