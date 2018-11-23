using System.Threading.Tasks;
using Domain.ViewModels;
using Infastructure.Result;

namespace Logic.Services
{
    public interface IBlockchainService
    {
        /// <summary>
        /// Создание нового кошелька
        /// </summary>
        /// <param name="password">пароль от контекстного аккаунта, будет подставлен в качестве пароля кошелька</param>
        Task<RequestResult<AccountViewModel>> CreateNewWalletAsync(string password);

        /// <summary>
        /// Получение баланса в USD для кошелька контекстного аккаунта
        /// </summary>
        /// <remarks>Результат меньше 0.01 usd (при курсе ~160usd = 1eth = ~100000000000000 wei) выводиться не будет</remarks>
        Task<RequestResult<BlockchainBalanceViewModel>> GetBalanceByCurrentAccountInUsdAsync();

        /// <summary>
        /// for test
        /// </summary>
        /// <param name="walletAddressTo">ignored</param>
        /// <returns></returns>
        Task<RequestResult<bool>> SendWeiAsync(string walletAddressTo, long value, string pass);

        /// <summary>
        /// Получение статуса транзакции
        /// </summary>
        /// <param name="transactionHash">хэш транзакции</param>
        /// <returns></returns>
        Task<string> GetTransactionsReceiptAsync(string transactionHash);

        /// <summary>
        /// Получение стоимости Eth в Usd на текущий момент времени
        /// </summary>
        /// <returns></returns>
        Task<RequestResult<int>> GetEthereumPriceInUsdAsync();
    }
}