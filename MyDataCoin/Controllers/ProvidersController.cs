using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace MyDataCoin.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class ProvidersController: ControllerBase
	{
		private readonly ILogger<BalanceController> _logger;

		public ProvidersController(ILogger<BalanceController> logger)
		{
			_logger = logger;
		}

        // TODO: реализовать контроллер для провайдеров

        /// <summary>
        /// Implement method
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Authorize]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> ImplementMethod(string userid)
        {
            throw new NotImplementedException();
        }
    }
}

