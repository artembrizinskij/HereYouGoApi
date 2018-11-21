using System.Threading.Tasks;
using Domain.Forms;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Security.Controllers;

namespace HereYouGoAPI.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : SecurityController
    {
        private readonly IAccountService _accountService;
        private readonly IBlockchainService _blockchainService;

        public AccountController(IAccountService accountService, IBlockchainService blockchainService) 
        {
            _accountService = accountService;
            _blockchainService = blockchainService;
        }

        [HttpPost]
        [Route("create")]
        public IActionResult CreateAccount(AccountForm form) => Ok(_accountService.CreateAccount(form));

        [HttpPost]
        [Route("auth")]
        public IActionResult Authorization(SignInForm form) => Ok(_accountService.SignIn(form));

        [HttpPost]
        [Authorize]
        [Route("wallet/create")]
        public async Task<IActionResult> CreateWalletAsync(string pass) => Ok(await _blockchainService.CreateNewWalletAsync(pass));

        [HttpGet]
        [Authorize]
        [Route("get/balance")]
        public async Task<IActionResult> GetBalance() => Ok( await _blockchainService.GetBalanceByCurrentAccountInUsdAsync());

        [HttpGet]
        [Authorize]
        [Route("profile/get")]
        public IActionResult GetProfile() => Ok(_accountService.GetCurrentAccount());

        [HttpGet]
        [Authorize]
        [Route("isAuthorized")]
        public IActionResult IsAuthorized() => Ok();
    }
}