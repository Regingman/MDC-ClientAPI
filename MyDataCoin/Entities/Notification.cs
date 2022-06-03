using System;
namespace MyDataCoin.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public string Image { get; set; }

        public DateTime NotificationDate { get; set; }

        public bool Read { get; set; }

        public Guid UserId { get; set; }
    }
}
