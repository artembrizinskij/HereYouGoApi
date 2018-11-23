using System.Threading.Tasks;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Security.Controllers;

namespace HereYouGoAPI.Controllers
{
    [Route("api/[controller]")]
    public class BlockchainController : SecurityController
    {
        private readonly IBlockchainService _blockchainService;

        public BlockchainController(IBlockchainService blockchainService)
        {
            _blockchainService = blockchainService;
        }

        [HttpPost]
        [Authorize]
        [Route("wei/send")]
        public async Task<IActionResult> SendWeiAsync(long value, string pass) => Ok(await _blockchainService.SendWeiAsync("", value, pass));

        [HttpPost]
        [Authorize]
        [Route("transactions/receipt")]
        public async Task<IActionResult> TransactionsReceiptAsync(string hash) => Ok(await _blockchainService.GetTransactionsReceiptAsync(hash));

        [HttpGet]
        [Route("eth/price/inUsd")]
        public async Task<IActionResult> GetEthPriceInUsdAsync() => Ok(await _blockchainService.GetEthereumPriceInUsdAsync());
    }
}
