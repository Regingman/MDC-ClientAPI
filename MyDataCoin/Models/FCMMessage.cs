using System;
namespace MyDataCoin.Models
{
    /// <summary>
    /// FCMMessage class for sending messages to the user 
    /// </summary>
    public class FCMMessage
    {
        /// <summary>
        /// message body
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// message title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// user id it is required to get the token
        /// </summary>
        public string UserId { get; set; }
    }
}

