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
        /// Add rewards to a wallet
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
        [HttpPost]
        [Route("add_to_balance/{userid}/amount/{amount}")]
        public async Task<IActionResult> Earn(string userid, double amount)
        {
            GeneralResponse response = await _BalanceService.AddToBalance(userid, amount);

            if (response.Code == 400)
                return StatusCode(400, new GeneralResponse(400, response.Message));
            else if (response.Code == 421)
                return StatusCode(421, new GeneralResponse(421, response.Message));
            else
                return Ok(response);
        }



        /// <summary>
        /// Get List of transactions (Directions: 1 - Send, 2 - Receive)
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<Entities.Transaction>))]
        [SwaggerResponse(421, Type = typeof(GeneralResponse))]
        [HttpGet("transactions/{userid}")]
        public IActionResult GetTransactions(string userid)
        {
            var result = _BalanceService.GetTransactions(userid);
            //if (result.Code == 421) return StatusCode(421, new GeneralResponse(421, "User Not Found"));
            //else return Ok(new GeneralResponse(200, "Success"));
            return Ok(result);
        }
    }
}
