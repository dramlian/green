namespace WalletApi.Models
{
    public class Transactions
    {
        public IEnumerable<Transaction>? transactions { get; set; }

        public Transactions(IEnumerable<Transaction> trasactionsvalue)
        {
            transactions = trasactionsvalue;
        }

        public Transactions()
        {
            transactions = new List<Transaction>();
        }
    }
}
