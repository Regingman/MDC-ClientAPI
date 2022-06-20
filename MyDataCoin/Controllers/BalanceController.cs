using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyDataCoin.Interfaces;
using MyDataCoin.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MyDataCoin.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BalanceController: ControllerBase
    {
        private readonly ILogger<BalanceController> _logger;
        private readonly IBalance _BalanceService;

        public BalanceController(ILogger<BalanceController> logger, IBalance BalanceService)
        {
            _BalanceService = BalanceService;
            _logger = logger;
        }


        /// <summary>
        /// Get user's current balance
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(AuthenticateResponse))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Type = typeof(GeneralResponse))]
        [SwaggerResponse(421, Type = typeof(GeneralResponse))]
        [Authorize]
        [HttpGet]
        [Route("get_balance/{userid}")]
        public async Task<IActionResult> GetBalance(string userid)
        {
            GeneralResponse response = await _BalanceService.GetBalance(userid);

            if (response.Code == 400)
                return StatusCode(400, new GeneralResponse(400, response.Message));
            else if (response.Code == 421)
                return StatusCode(421, new GeneralResponse(421, response.Message));
            else
                return Ok(response);
        }




        /// <summary>
        /// Get List of transactions(Send - if your address is Address Sender, Receive - if your address is Address Receiver)
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        //[Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<Entities.Transaction>))]
        [SwaggerResponse(421, Type = typeof(GeneralResponse))]
        [HttpGet("transactions/{userid}")]
        public async Task<IActionResult> GetTransactions(string userid)
        {
            var result = await _BalanceService.GetTransactions(userid);
            //if (result.Code == 421) return StatusCode(421, new GeneralResponse(421, "User Not Found"));
            //else return Ok(new GeneralResponse(200, "Success"));
            return Ok(result);
        }



        /// <summary>
        /// Add rewards for watching Ads
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(AuthenticateResponse))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Type = typeof(GeneralResponse))]
        [SwaggerResponse(421, Type = typeof(GeneralResponse))]
        [Authorize]
        [HttpGet]
        [Route("advertising_rewards/{userid}")]
        public async Task<IActionResult> Earn(string userid)
        {
            GeneralResponse response = await _BalanceService.AdvertisingRewards(userid);

            if (response.Code == 400)
                return StatusCode(400, new GeneralResponse(400, response.Message));
            else if (response.Code == 421)
                return StatusCode(421, new GeneralResponse(421, response.Message));
            else
                return Ok(response);
        }




        /// <summary>
        /// Add rewards to users who input promo code
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="426">Returns You already reffered</response>
        /// <response code="500">Returns Internal Server Error</response>
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GeneralResponse))]
        [SwaggerResponse(421, Type = typeof(GeneralResponse))]
        [SwaggerResponse(426, Type = typeof(GeneralResponse))]
        [HttpGet("promocode_rewards/{userid}/{promo}")]
        public async Task<IActionResult> InsertPromo(string userid, string promo)
        {
            var result = await _BalanceService.PromoCodeRewards(userid, promo);
            if (result.Code != 200) return StatusCode(result.Code, new GeneralResponse(result.Code, result.Message));
            else return Ok(new GeneralResponse(result.Code, result.Message));
        }



        /// <summary>
        /// Send assets from one address to another, should be email or wallet address
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="422">Returns Not Enough Funds</response>
        /// <response code="424">Returns Amount should be more than 0</response>
        /// <response code="427">Returns Recepient Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GeneralResponse))]
        [SwaggerResponse(421, Type = typeof(GeneralResponse))]
        [SwaggerResponse(422, Type = typeof(GeneralResponse))]
        [SwaggerResponse(424, Type = typeof(GeneralResponse))]
        [SwaggerResponse(427, Type = typeof(GeneralResponse))]
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendModel model)
        {
            if (model.Amount <= 0) return StatusCode(424, new GeneralResponse(424, "Amount should be more than 0"));
            var result = await _BalanceService.Send(model);
            if (result.Code == 200) return Ok(new GeneralResponse(result.Code, result.Message));
            else return StatusCode(result.Code, new GeneralResponse(result.Code, result.Message));
        }




        /// <summary>
        /// Validate wallet address
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="500">Returns Internal Server Error</response>
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [HttpGet("validate/{address}")]
        public IActionResult Validate(string address)
        {
            if (string.IsNullOrEmpty(address)) return BadRequest("Address cannot be null");
            else
            {
                var result = _BalanceService.Validate(address);
                return Ok(result);
            }
        }
    }
}
