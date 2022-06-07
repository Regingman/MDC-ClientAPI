using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyDataCoin.DataAccess;
using MyDataCoin.Interfaces;
using MyDataCoin.Models;

namespace MyDataCoin.Services
{
    public class BalanceService: IBalance
    {
        private readonly WebApiDbContext _db;

        public BalanceService(WebApiDbContext db)
        {
            _db = db;
        }

        public async Task<GeneralResponse> GetBalance(string userid)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == Guid.Parse(userid));
            return user != null ? new GeneralResponse(200, user.Balance.ToString()) : new GeneralResponse(421, "User Not Found");
        }

        public async Task<List<Entities.Transaction>> GetTransactions(string userid)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == Guid.Parse(userid));
            if (user == null) return new List<Entities.Transaction>();
            else return await _db.Transactions.Where(x => x.TxId == user.Id).ToListAsync();
        }

        public async Task<GeneralResponse> AddToBalance(string id, double amount)
        {
            try
            {
                Guid guid = Guid.Parse(id);
                var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == guid);
                var mdcWallet = await _db.Users.SingleOrDefaultAsync(x => x.Id == Guid.Parse("21d2c0d3-ec2c-4601-9b19-df7ac7aa2da5"));
                if (mdcWallet.Balance < 100) return new GeneralResponse(400, "Not enough funds in MDC marketing wallet");
                else
                {
                    if (user != null)
                    {
                        mdcWallet.Balance -= amount;
                        user.Balance = user.Balance += amount;

                        Entities.Transaction transaction = new
                            Entities.Transaction()
                        {
                            TxId = Guid.NewGuid(),
                            From = mdcWallet.Id,
                            To = user.Id,
                            Amount = amount,
                            AmountInUsd = amount - 0.5,
                            TxDate = DateTime.Now,
                            Direction = 2
                        };

                        await _db.AddAsync(transaction);
                        await _db.SaveChangesAsync();
                        return new GeneralResponse(200, "Success");
                    }
                    else
                    {
                        return new GeneralResponse(421, "User Not Found");
                    }
                }
            }
            catch (Exception ex)
            {
                return new GeneralResponse(400, ex.Message);
            }
        }
    }
}
