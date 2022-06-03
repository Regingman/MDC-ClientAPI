using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoin.Entities
{
    public class Email
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string EmailAddress { get; set; }

        public DateTime CreatedAt { get; set; }

        public string SocialNetworkName { get; set; }
    }
}
