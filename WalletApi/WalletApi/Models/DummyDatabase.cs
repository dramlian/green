namespace WalletApi.Models
{
    public class DummyDatabase
    {
        public List<Player> players { get; set; }

        public DummyDatabase()
        {
            players = new List<Player>() {
                new Player("Peter",50),
                new Player("John",40),
                new Player("Bob",42),
                new Player("Alice",25)
            };
        }
    }
}
