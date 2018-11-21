using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DAl.Sql.Services;
using Domain.Structs.Web3Service;
using Domain.ViewModels;
using Infastructure.Extensions;
using Infastructure.Helpers;
using Infastructure.Result;
using Newtonsoft.Json;
using Security.Providers;

namespace Logic.Services
{
    public class Web3Service : IBlockchainService
    {
        private readonly IContextProvider _contextProvider;
        private readonly ICommonDbService _db;
        private static readonly HttpClient Client = new HttpClient();
        private string hostAddress = "http://192.168.1.148:9000";
        private const long OneEthInWei = 1000000000000000000;

        public Web3Service(IContextProvider contextProvider, ICommonDbService commonDbService)
        {
            _contextProvider = contextProvider;
            _db = commonDbService;
        }

        public async Task<RequestResult<AccountViewModel>> CreateNewWalletAsync(string password)
        {
            var result = new RequestResult<AccountViewModel>();
            var account = _contextProvider.Account;
            if (account == null)
                return result.AddError("Account empty");
            if (!PasswordHelper.IsMatch(password, account.Password))
                return result.AddError("Incorrect password");
            var content = new FormUrlEncodedContent(new Dictionary<string, string>{{ "pass", password }});
            var response = await Client.PostAsync(CreateUrl("/account/create"), content);
            var responseString = await response.Content.ReadAsStringAsync();
            var deserializeObject = JsonConvert.DeserializeObject<SimpleResult>(responseString);
            if (deserializeObject.Result.IsNullOrEmpty())
                return result.AddError("Error creating wallet");
            account.WalletAddress = deserializeObject.Result;
            _db.Accounts.Update(account);
            _db.Commit();
            return result.SetData(account.MapTo<AccountViewModel>());
        }

        public async Task<RequestResult<decimal>> GetBalanceByCurrentAccountInUsdAsync()
        {
            var result = new RequestResult<decimal>();

            //42 - стандартная длина адресса в blockchain
            if (_contextProvider.Account.WalletAddress.IsNullOrEmpty() || _contextProvider.Account.WalletAddress.Length != 42)
                return result.AddError("Incorrect wallet address");

            var values = new Dictionary<string, string>{ { "account", _contextProvider.Account.WalletAddress } };
            var response = await SendPostRequestAsync(CreateUrl("/getBalance") , values);
            var usd = await GetEthPriceInUsdAsync();
            var deserializeObject = JsonConvert.DeserializeObject<SimpleResult>(response);

            if (!long.TryParse(deserializeObject.Result, out long lonV))
                return result.AddError("Incorrect value");

            return result.SetData((lonV / OneEthInWei) * usd);
        }

        public async Task<RequestResult<bool>> SendWeiAsync(string walletAddressTo, long value, string pass)
        {
            var result = new RequestResult<bool>();
            //var content = new Dictionary<string, string>(){{"from", _contextProvider.Account.WalletAddress}, {"to", walletAddressTo}};
            var content = new Dictionary<string, string>()
            {
                { "from", "0x07e605e7431e473e435d2e35f0c7c50590684335" }, { "to", _contextProvider.Account.WalletAddress },
                {"value", $"{value}" }
            };
            var response = await SendPostRequestAsync(CreateUrl("/ether/send"), content, pass);
            var res = JsonConvert.DeserializeObject<SimpleResult>(response);

            var status = await GetTransactionsReceiptAsync(res.Result);
            if (status == "0x0")
                return result.AddError($"Invalid transactions status: {status}. Check status later");

            return result.SetData(true);
        }

        public async Task<string> GetTransactionsReceiptAsync(string transactionHash)
        {
            var content = new Dictionary<string, string>() { { "hash", transactionHash } };
            var response = await SendPostRequestAsync(CreateUrl("/transactions/receipt"), content);
            return JsonConvert.DeserializeObject<TransactionsResult>(response).Result.Status;
        }

        private async Task<int> GetEthPriceInUsdAsync()
        {
            var res = await Client.GetAsync("https://api.coinmarketcap.com/v1/ticker/ethereum/");
            var responseString = await res.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<List<EthPrices>>(responseString).FirstOrDefault().price_usd.Split('.').FirstOrDefault();
            int.TryParse(obj, out int value);
            return value;
        }

        private async Task<string> SendPostRequestAsync(string url, Dictionary<string, string> contentArray, string pass = "")
        {
            var content = new FormUrlEncodedContent(contentArray);
            if (pass != "")
            {
                var responseLogin = await Client.PostAsync(url, new FormUrlEncodedContent(new Dictionary<string, string>(){
                    { "pass", pass},
                    { "account", "0x07e605e7431e473e435d2e35f0c7c50590684335" } }));
                var signResp = await responseLogin.Content.ReadAsStringAsync();
            }

            var response = await Client.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }

        private string CreateUrl(string path)
        {
            if (path.IsNullOrEmpty())
                return path;
            if (path.ToCharArray()[0] != '/')
                path = $"/{path}";
            return hostAddress + path;
        }
    }
}
