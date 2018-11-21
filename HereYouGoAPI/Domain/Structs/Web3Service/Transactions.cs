using System.Collections.Generic;

namespace Domain.Structs.Web3Service
{
    public struct Transactions
    {
        public string Status { get; set; }
        public string BlockHash { get; set; }
        public int BlockNumber { get; set; }
        public string ContractAddress { get; set; }
        public int CumulativeGasUsed { get; set; }
        public string From { get; set; }
        public int GasUsed { get; set; }
        public IEnumerable<string> Logs { get; set; }
        public string LogsBloom { get; set; }
        public string To { get; set; }
        public string TransactionHash { get; set; }
        public int TransactionIndex { get; set; }
    }
}