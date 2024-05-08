using WalletApi.Models;

namespace WalletApi.Helper
{
    public class WalletHelper
    {
        private Wallet _wallet {  get; set; }

        public WalletHelper(Wallet wallet)
        {
            _wallet=wallet;
        }

        public void ChangeWalletBalance(decimal value)
        {
            _wallet.balance += value;
            bool wasValid = ValidateBalance();
            AddTransactionToHistory(value);
            ThrowIfInvalidOperation(wasValid, value);
        }

        private void ThrowIfInvalidOperation(bool wasValid, decimal value)
        {
            if (wasValid) return;
            throw new InvalidOperationException($"Balance can not be " +
                $"negative ammount, you entered {value} and the balance" +
                $" is {_wallet.balance}");
        }

        public decimal GetBalance()
        {
            return _wallet.balance;
        }

        public bool ValidateBalance()
        {
            if (_wallet.balance < 0)
            {
                _wallet.balance = 0;
                return false;
            }
            return true;
        }

        public void AddTransactionToHistory(decimal value)
        {
            _wallet.transactionHistory.Add(new Transaction(value, _wallet.balance, DateTime.Now));
        }

        public Transactions GetTransactions()
        {
            return new Transactions(_wallet.transactionHistory);
        }

    }
}
