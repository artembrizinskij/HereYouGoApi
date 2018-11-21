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
        Task<RequestResult<decimal>> GetBalanceByCurrentAccountInUsdAsync();

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
    }
}