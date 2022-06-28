using System;
using System.Threading.Tasks;
using MyDataCoin.Models;

namespace MyDataCoin.Interfaces
{
	public interface IProviders
	{
		public Task<string> GetMashinaKgUserAsync(string email);

		public Task<string> GetDataFromGoogleAsync(string jwtToken);

		public Task<string> GetDataFromFacebookAsync(string jwtToken);

		// TODO: реализовать модель истории монетизации
		public Task<MonetizationModel> GetTotalMonetizationStatAsync(string email);
	}
}

