using FirebaseAdmin.Messaging;
using MyDataCoin.Models;
using System.Threading.Tasks;

namespace MyDataCoin.Interfaces
{
    public interface IFireBase
    {
        public Task<AuthenticateResponse> SendNotification(FCMMessage message);
    }
}
