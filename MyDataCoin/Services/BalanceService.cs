using System;
using System.Collections.Generic;
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

        public List<Entities.Transaction> GetTransactions(string id)
        {
            List<Entities.Transaction> transactions = new List<Entities.Transaction>();

            Entities.Transaction transaction0 = new Entities.Transaction(
                "A7C52681C168C9C6F4650C98E9DB31E96692A3630E468869C93C7C062A5FCCFA",
                "mdc18ld4633yswcyjdklej3att6aw93nhlf7ce4v8u",
                "mdc15e0ttksn43pt22tej3mkes77evn95m76hgf8yx", 0.02, 0.0004, DateTime.Now, 1
            );

            Entities.Transaction transaction1 = new Entities.Transaction(
                "C4C3CA6A607295532C6C206C551D7DF68CAD29D5ABCA060E1EDDAB45500209AB",
                "cosmos15vdjje8009ly9rudapxjcht5h9vshc7hx2ww5k",
                "cosmos15v50ymp6n5dn73erkqtmq0u8adpl8d3ujv2e74", 120.02, 590.33, DateTime.Now, 2
            );

            Entities.Transaction transaction2 = new Entities.Transaction(
                "E7653467CEC855BD7F3D0BE3169F1B827AB3FF6163D9251F4E5C4A46C28542C3",
                "cosmos1g5ktscf6794zju2fe480nwu5xdrhp2mqx45cz9",
                "mdc15e0ttksn43pt22tej3mkes77evn95m76hgf8yx", 15000580.02, 9756750000.22, DateTime.Now, 1
            );

            Entities.Transaction transaction3 = new Entities.Transaction(
                "F6TI59TIR9C855BD7F3D0BE3169F1B827AB3FF6163D9251F4E5C4A46C28542C3",
                "mdc1g5ktscf6794zju2fe480nwu5xdrhp2mqx45cz9",
                "mdc15e0ttksn43pt22tej3mkes77evn95m76hgf8yx", 43.04325, 560.54, DateTime.Now, 1
            );

            transactions.Add(transaction0);
            transactions.Add(transaction1);
            transactions.Add(transaction2);
            transactions.Add(transaction3);

            return transactions;
        }

        public async Task<GeneralResponse> AddToBalance(string id, double amount)
        {
            try
            {
                Guid guid = Guid.Parse(id);
                var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == guid);
                if (user != null)
                {
                    user.Balance = user.Balance += amount;
                    await _db.SaveChangesAsync();
                    return new GeneralResponse(200, "Success");
                }
                else
                {
                    return new GeneralResponse(421, "User Not Found");
                }
            }
            catch (Exception ex)
            {
                return new GeneralResponse(400, ex.Message);
            }
        }
    }
}
