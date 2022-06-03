using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyDataCoin.Entities
{
    public class EmailCodeDictionary
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Code { get; set; }

        public bool IsVerified { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
