using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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


	}
}

