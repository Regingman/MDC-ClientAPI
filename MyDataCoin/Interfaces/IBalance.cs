using System.Collections.Generic;
using System.Threading.Tasks;
using MyDataCoin.Models;

namespace MyDataCoin.Interfaces
{
    public interface IBalance
    {
        Task<GeneralResponse> GetBalance(string userid);

        Task<List<Entities.Transaction>> GetTransactions(string id);

        Task<GeneralResponse> AdvertisingRewards(string id);

        Task<GeneralResponse> PromoCodeRewards(string userid, string promo);

        Task<GeneralResponse> Send(SendModel model);

        bool Validate(string address);
    }
}
