using System;

namespace Domain.ViewModels
{
    public class AccountViewModel
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string WalletAddress { get; set; }
    }
}
