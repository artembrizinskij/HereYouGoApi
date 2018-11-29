using Domain.Forms;
using Domain.ViewModels;
using Infastructure.Result;

namespace Logic.Services
{
    public interface IAccountService
    {
        RequestResult<bool> CreateAccount(AccountForm form);
        RequestResult<JsonWebTokenViewModel> SignIn(SignInForm form);
        RequestResult<bool> UpdateAccount(AccountForm form);
        RequestResult<AccountViewModel> GetCurrentAccount();
    }
}