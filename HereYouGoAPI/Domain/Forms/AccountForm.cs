using Domain.Abstractions;

namespace Domain.Forms
{
    public class AccountForm : SignInForm
    {
        public string VerificationPassword { get; set; }
        /// <summary>
        /// Адресс кошелька в blockchain
        /// </summary>
        public string WalletAddress { get; set; }
    }

    public class SignInForm : IForm
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
