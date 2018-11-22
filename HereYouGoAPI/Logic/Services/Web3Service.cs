﻿using System;
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

        public async Task<RequestResult<decimal>> GetBalanceByCurrentAccountInUsdAsync()
        {
            var result = new RequestResult<decimal>();

            //42 - стандартная длина адресса в blockchain
            if (_contextProvider.Account.WalletAddress.IsNullOrEmpty() || _contextProvider.Account.WalletAddress.Length != 42)
                return result.AddError("Incorrect wallet address");

            var content = new Dictionary<string, string>{ { "account", _contextProvider.Account.WalletAddress } };

            //mock result for demo test
            var accountBalanceInWeiMock = 1000000000000000000;
            //var response = await SendPostRequestAsync(CreateUrl("/getBalance") , content);
            var usd = await GetEthPriceInUsdAsync();

            //if (!response.TryDeserializeObject(out SimpleResult deserializeObject))
            //    return result.AddError("Error response");

            //if (!long.TryParse(deserializeObject.Result, out long lonV))
            //    return result.AddError("Incorrect value");
            
            return result.SetData(Math.Round((decimal)((accountBalanceInWeiMock / OneEthInWei) * usd), 2));
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
