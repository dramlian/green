using WalletApi.Models;

namespace WalletApi.Helper
{
    public class PlayerHelper
    {
        private Player _player { get; set; }

        private WalletHelper? _walletHelper { get; set; }

        public PlayerHelper(Player player)
        {
            _player = player;
            if (player.wallet != null)
            {
                _walletHelper = new WalletHelper(player.wallet);
            }
        }

        public decimal GetUserBalance()
        {
            return _walletHelper?.GetBalance() ?? 0;
        }

        public Transactions GetUserTrasnactions()
        {
            return _walletHelper?.GetTransactions() ?? new Transactions();
        }

        public void EditUserBalance(decimal value)
        {
            _walletHelper?.ChangeWalletBalance(value);
        }
    }
}
