using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyDataCoin.DataAccess;
using MyDataCoin.Entities;
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
            else
            {
                List<Transaction> res = await _db.Transactions
                    .Where(x => x.From == user.WalletAddress)
                    .Where(y => y.To == user.WalletAddress)
                    .ToListAsync();
                return res;
            }
        }

        public async Task<GeneralResponse> AdvertisingRewards(string userid)
        {
            try
            {
                Guid guid = Guid.Parse(userid);
                var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == guid);
                var mdcWallet = await _db.Users.SingleOrDefaultAsync(x => x.Id == Guid.Parse("21d2c0d3-ec2c-4601-9b19-df7ac7aa2da5"));
                if (mdcWallet.Balance < 100) return new GeneralResponse(400, "Not enough funds in MDC marketing wallet");
                else
                {
                    if (user != null)
                    {
                        var transaction = await MakeTransfer(mdcWallet, user, 0.5, (int) TransactionType.Advertising);
                        if (transaction)
                            return new GeneralResponse(200, "Success");
                        else
                            return new GeneralResponse(400, "Create transaction error");
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


        public async Task<GeneralResponse> PromoCodeRewards(string userid, string promo)
        {
            Entities.User newUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userid));
            Entities.User invitingUser = await _db.Users.FirstOrDefaultAsync(u => u.RefCode == promo);
            Entities.User mdcWallet = await _db.Users.SingleOrDefaultAsync(x => x.Id == Guid.Parse("21d2c0d3-ec2c-4601-9b19-df7ac7aa2da5"));

            if (invitingUser == null) return new GeneralResponse(400, "Invalid Promo Code");
            else if (newUser.CameFrom != null) return new GeneralResponse(426, "You already reffered");
            else if (newUser.CameFrom == newUser.RefCode) return new GeneralResponse(400, "You cannot invite yourself");
            else
            {
                if (mdcWallet.Balance < 100) return new GeneralResponse(400, "Not enough funds in MDC marketing wallet");
                else
                {
                    newUser.CameFrom = promo;

                    var toInviter = await MakeTransfer(mdcWallet, invitingUser, 2.5, (int)TransactionType.Refferal);
                    var toNewUser = await MakeTransfer(mdcWallet, newUser, 2.5, (int)TransactionType.Refferal);

                    if (toInviter == true && toNewUser == true)
                        return new GeneralResponse(200, "Success");
                    else
                        return new GeneralResponse(400, "Create transaction error");
                }
            }

        }



        public async Task<GeneralResponse> Send(SendModel model)
        {
            var userFrom = await _db.Users.SingleOrDefaultAsync(x => x.WalletAddress == model.AddressFrom);
            var userTo = await _db.Users.SingleOrDefaultAsync(x => x.WalletAddress == model.AddressTo);

            if (userFrom == null || userTo == null) return new GeneralResponse(421, "User Not Found");
            else
            {
                if (userFrom.Balance < model.Amount) return new GeneralResponse(424, "Not Enough Funds");
                else
                {
                    var tx = await MakeTransfer(userFrom, userTo, model.Amount, (int)TransactionType.Send);
                    if (tx) return new GeneralResponse(200, "Success");
                    else return new GeneralResponse(400, "Create transaction error");
                }
            }    
        }

       
        public bool Validate(string address)
        {
            var regexItem = new Regex("^[a-zA-Z0-9 ]*$");

            if (address.Length < 20) return false;
            if (address[0] != 'm' && address[1] != 'd' && address[2] != 'c') return false;
            if (!regexItem.IsMatch(address)) return false;

            return true;
        }

        //Helper method
        private async Task<bool> MakeTransfer(User from, User to, double amount, int type)
        {
            try
            {
                Entities.Transaction transaction = new Entities.Transaction()
                {
                    TxId = Guid.NewGuid(),
                    From = from.WalletAddress,
                    To = to.WalletAddress,
                    Amount = amount,
                    TxDate = DateTime.Now,
                    Type = type
                };

                from.Balance -= amount;
                to.Balance += amount;

                _db.Transactions.Add(transaction);
                await _db.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
