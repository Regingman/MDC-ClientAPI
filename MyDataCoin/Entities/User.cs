using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyDataCoin.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [JsonIgnore]
        public string FacebookId { get; set; }

        [JsonIgnore]
        public string GoogleId { get; set; }

        [JsonIgnore]
        public string AppleId { get; set; }

        public string NickName { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt { get; set; }

        public string ProfilePic { get; set; }

        public double Balance { get; set; }

        public string WalletAddress { get; set; }

        [JsonIgnore]
        public string RefCode { get; set; }

        [JsonIgnore]
        public string CameFrom { get; set; }
    }
}
