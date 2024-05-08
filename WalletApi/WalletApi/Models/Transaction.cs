namespace WalletApi.Models
{
    public class Transaction
    {
        public Guid id { get; }
        public decimal resultingAmmount {  get; }
        public decimal transactionValue { get; }
        public DateTime? transactionDate { get; }

        public Transaction(decimal transactionValue, decimal resultingAmmount, DateTime transactionDate)
        {
            id = Guid.NewGuid();
            this.resultingAmmount = resultingAmmount;
            this.transactionValue= transactionValue;
            this.transactionDate = transactionDate;
        }
    }
}
