using System;
using System.Threading.Tasks;
using MyDataCoin.Interfaces;
using MyDataCoin.Models;

namespace MyDataCoin.Services
{
	public class ProvidersServices: IProviders
	{
		public ProvidersServices()
		{
		}

        public Task<string> GetDataFromFacebookAsync(string jwtToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetDataFromGoogleAsync(string jwtToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetMashinaKgUserAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<MonetizationModel> GetTotalMonetizationStatAsync(string email)
        {
            throw new NotImplementedException();
        }

        // TODO: реализовать сервис
    }
}

