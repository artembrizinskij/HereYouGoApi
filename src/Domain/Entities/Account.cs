using Domain.Forms;
using Infastructure.Helpers;

namespace Domain.Entities
{
    public class Account : Entity
    {
        protected Account() { }
        public Account(AccountForm form)
        {
            Login = form.Login;
            Password = PasswordHelper.Generate(form.Password);
            WalletAddress = form.WalletAddress;
        }

        public string Login { get; set; }
        public string Password { get; set; }
        public string WalletAddress { get; set; }

        public bool UpdateAccount(AccountForm form)
        {
            var isUpdated = false;
            if (form.WalletAddress != WalletAddress)
            {
                WalletAddress = form.WalletAddress;
                isUpdated = true;
            }

            return isUpdated;
        }
    }
}
