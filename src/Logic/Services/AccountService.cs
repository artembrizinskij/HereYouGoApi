using System;
using Domain.Forms;
using DAl.Sql.Services;
using Domain.Entities;
using Domain.ViewModels;
using Infastructure.Extensions;
using Infastructure.Helpers;
using Infastructure.Result;
using Security.Providers;

namespace Logic.Services
{
    public class AccountService : IAccountService
    {
        private readonly ICommonDbService _db;
        private readonly ITokenProvider _tokenProvider;
        private readonly IContextProvider _contextProvider;

        public AccountService(ICommonDbService db, ITokenProvider tokenProvider, IContextProvider contextProvider)
        {
            _db = db;
            _tokenProvider = tokenProvider;
            _contextProvider = contextProvider;
        }

        public RequestResult<bool> CreateAccount(AccountForm form)
        {
            var result = new RequestResult<bool>();

            if (form.Login.IsNullOrEmpty() || form.Password.IsNullOrEmpty() || form.VerificationPassword.IsNullOrEmpty())
                return result.AddError("Not all form fields are filled in");
            if (form.Password != form.VerificationPassword)
                return result.AddError("Password field does Not match verification field");

            _db.Accounts.AddOne(new Account(form));
            _db.Commit();

            return result;
        }

        public RequestResult<JsonWebTokenViewModel> SignIn(SignInForm form)
        {
            var result = new RequestResult<JsonWebTokenViewModel>();
            var account = _db.Accounts.FindOne(x => x.Login == form.Login);
            if (account == null || !PasswordHelper.IsMatch(form.Password, account.Password))
                return result.AddError("Authorization error");

            var expireMinuts = 1200;
            return result.SetData(new JsonWebTokenViewModel()
            {
                AccessToken = _tokenProvider.CreateToken(account, DateTime.UtcNow.AddMinutes(expireMinuts)),
                Expire = expireMinuts
            });
        }

        public RequestResult<bool> UpdateAccount(AccountForm form)
        {
            var result = new RequestResult<bool>();
            if (_contextProvider.Account == null)
                return result.AddError("Empty account");

            if (_contextProvider.Account.UpdateAccount(form))
            {
                _db.Accounts.Update(_contextProvider.Account);
                _db.Commit();
            }

            return result;
        }

        public RequestResult<AccountViewModel> GetCurrentAccount()
        {
            return new RequestResult<AccountViewModel>().SetData(_contextProvider.Account.MapTo<AccountViewModel>());
        }
            
    }
}
