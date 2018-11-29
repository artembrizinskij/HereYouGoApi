using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DAl.Sql.Services;
using Domain.Structs.Web3Service;
using Domain.ViewModels;
using Infastructure.Extensions;
using Infastructure.Helpers;
using Infastructure.Result;
using Microsoft.Extensions.Configuration;
using Security.Providers;

namespace Logic.Services
{
    public class Web3Service : IBlockchainService
    {
        private readonly IContextProvider _contextProvider;
        private readonly ICommonDbService _db;
        private static readonly HttpClient Client = new HttpClient();
        private readonly string _hostAddress;
        private const double OneEthInWei = 1000000000000000000.0;
        private const string ContractAddressTest = "0xfb1fb9f5f3c93c08f8f522f3541f7b4998497875";

        public Web3Service(IContextProvider contextProvider, ICommonDbService commonDbService, IConfiguration configuration)
        {
            _contextProvider = contextProvider;
            _db = commonDbService;
            _hostAddress = configuration.GetSection("Hosts:Web3HostAddress").Value ?? "";
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

            if (!responseString.TryDeserializeObject(out SimpleResult deserializeObject))
                return result.AddError("Error response");

            if (deserializeObject.Result.IsNullOrEmpty())
                return result.AddError("Error creating wallet");

            account.WalletAddress = deserializeObject.Result;

            _db.Accounts.Update(account);
            _db.CommitAsync();
            return result.SetData(account.MapTo<AccountViewModel>());
        }

        public async Task<RequestResult<BlockchainBalanceViewModel>> GetBalanceByCurrentAccountInUsdAsync()
        {
            var result = new RequestResult<BlockchainBalanceViewModel>();

            //42 - стандартная длина адресса в blockchain
            //if (_contextProvider.Account.WalletAddress.IsNullOrEmpty() || _contextProvider.Account.WalletAddress.Length != 42)
            //    return result.AddError("Incorrect wallet address");

            //var content = new Dictionary<string, string>{ { "account", _contextProvider.Account.WalletAddress } };

            //mock result for demo test
            var accountBalanceInWeiMock = 1000000000000000000;
            //var response = await SendPostRequestAsync(CreateUrl("/getBalance") , content);
            var usd = await GetEthPriceInUsdAsync();

            //if (!response.TryDeserializeObject(out SimpleResult deserializeObject))
            //    return result.AddError("Error response");

            //if (!long.TryParse(deserializeObject.Result, out long weiValue))
            //    return result.AddError("Incorrect value");
            var ethValue = (decimal)(accountBalanceInWeiMock / OneEthInWei);
            var inUsd = Math.Round(ethValue * usd, 2);
            return result.SetData(new BlockchainBalanceViewModel(inUsd, ethValue, accountBalanceInWeiMock));

        }

        public async Task<RequestResult<bool>> SendWeiAsync(string walletAddressTo, long value, string pass)
        {
            var result = new RequestResult<bool>();
            var content = new Dictionary<string, string>() { { "from", _contextProvider.Account.WalletAddress }, { "to", walletAddressTo } };
            var response = await SendPostRequestAsync(CreateUrl("/ether/send"), content);
            if (!response.TryDeserializeObject(out SimpleResult deserializeObject))
                return result.AddError("Error response");

            var status = await GetTransactionsReceiptAsync(deserializeObject.Result);
            if (status == "0x0")
                return result.AddError($"Invalid transactions status: {status}. Check status later");

            return result.SetData(true);
        }

        public async Task<string> GetTransactionsReceiptAsync(string transactionHash)
        {
            var response = await SendPostRequestAsync(CreateUrl("/transactions/receipt"), new Dictionary<string, string>() { { "hash", transactionHash } });
            if (!response.TryDeserializeObject(out TransactionsResult deserializeObject))
                return "Error response";
            return deserializeObject.Result.Status;
        }

        public async Task<RequestResult<int>> GetEthereumPriceInUsdAsync() => new RequestResult<int>().SetData(await GetEthPriceInUsdAsync());

        #region SmartContractMethods

        public async Task<RequestResult<bool>> BuyCarShares(int amountUsd, string walletPassword)
        {
            var result = new RequestResult<bool>();
            var balanceInUsdOfCurrentAccount = await GetBalanceByCurrentAccountInUsdAsync();

            if (amountUsd > balanceInUsdOfCurrentAccount.Data.UsdValue)
                return result.AddError("Not enough money");

            var priceEthInUsd = await GetEthPriceInUsdAsync();
            var amountInWei = (long)((decimal)(OneEthInWei / 100) * ((amountUsd / ((decimal)priceEthInUsd / 100))));

            var response = await SendPostRequestAsync(CreateUrl("/contract/smartcar/function/buyCarShares"), new Dictionary<string, string>()
            {
                { "address", ContractAddressTest },
                { "account", _contextProvider.Account.WalletAddress },
                { "pass", walletPassword },
                { "amountUsd", $"{amountUsd}" },
                { "amountWei", $"{amountInWei}" }
            });

            if (!response.TryDeserializeObject(out SimpleResult deserializeObject))
                return result.AddError("Error response");

            return result;
        }

        #endregion

        private async Task<int> GetEthPriceInUsdAsync()
        {
            var response = await Client.GetAsync("https://api.coinmarketcap.com/v1/ticker/ethereum/");
            var responseString = await response.Content.ReadAsStringAsync();
            if (!responseString.TryDeserializeObject(out List<EthPrices> deserializeObject))
                return 0;
            var obj = deserializeObject.FirstOrDefault().price_usd.Split('.').FirstOrDefault();
            int.TryParse(obj, out int value);
            return value;
        }

        private async Task<string> SendPostRequestAsync(string url, Dictionary<string, string> contentArray)
        {
                var response = await Client.PostAsync(url, new FormUrlEncodedContent(contentArray));
                return await response.Content.ReadAsStringAsync();
        }

        private string CreateUrl(string path)
        {
            if (path.IsNullOrEmpty())
                return path;
            if (path.ToCharArray()[0] != '/')
                path = $"/{path}";
            return _hostAddress + path;
        }
    }
}
