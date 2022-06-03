using System.Collections.Generic;
using System.Threading.Tasks;
using MyDataCoin.Models;

namespace MyDataCoin.Interfaces
{
    public interface IBalance
    {
        Task<GeneralResponse> GetBalance(string userid);

        List<Entities.Transaction> GetTransactions(string id);

        Task<GeneralResponse> AddToBalance(string id, double amount);
    }
}
