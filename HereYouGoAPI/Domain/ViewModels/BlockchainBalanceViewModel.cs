using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ViewModels
{
    public class BlockchainBalanceViewModel
    {
        public BlockchainBalanceViewModel() { }
        public BlockchainBalanceViewModel(decimal usd, decimal eth, long wei)
        {
            UsdValue = usd;
            EthValue = eth;
            WeiValue = wei;
        }

        public decimal UsdValue { get; set; }
        public decimal EthValue { get; set; }
        public long WeiValue { get; set; }
    }
}
