using System.Data;
using WalletApi.Helper;

namespace WalletApi.Models
{
    public class Player
    {
        private Guid _id { get; }
        private string _name { get; }
        public string fullUniqueName { get; }
        public Wallet? wallet { get; set; } 

        public Player(string name, decimal initialBalance=0)
        {
            _id = Guid.NewGuid();
            _name = name;
            fullUniqueName = $"{name}_{_id}";
            wallet = new Wallet(fullUniqueName, initialBalance);
            var walletHelper = new WalletHelper(wallet);
            walletHelper.ValidateBalance();
            walletHelper.AddTransactionToHistory(initialBalance);
        }

        public Player(string name)
        {
            _id = Guid.NewGuid();
            _name = name;
            fullUniqueName = $"{name}_{_id}";
        }
    }
}
