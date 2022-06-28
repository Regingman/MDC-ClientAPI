using System;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using MyDataCoin.Models;

namespace MyDataCoin.Interfaces
{
    public interface INotification
    {
        public Task<GeneralResponse> SendNotification(FCMMessage message);
    }
}
