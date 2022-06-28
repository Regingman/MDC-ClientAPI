using System;
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
    public class NotificationsController: ControllerBase
    {
        private readonly ILogger<NotificationsController> _logger;
        private readonly INotification _notification;

        public NotificationsController(ILogger<NotificationsController> logger, INotification notification)
        {
            _notification = notification;
            _logger = logger;
        }

        /// <summary>
        /// Send message to user
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="404">Returns Not Found</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(AuthenticateResponse))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Type = typeof(GeneralResponse))]
        [SwaggerResponse(421, Type = typeof(GeneralResponse))]
        //[Authorize]
        [HttpPost]
        [Route("SendMessageToUser")]
        public async Task<IActionResult> SendMessageToUserAsync(FCMMessage message)
        {
            var response = await _notification.SendNotification(message);
            switch (response.Code)
            {
                case 400: return BadRequest(response.Message);
                case 421: return StatusCode(421, response.Message);
                case 200: return Ok(response.Message);
                default: return NotFound();
            }
        }
    }
}
