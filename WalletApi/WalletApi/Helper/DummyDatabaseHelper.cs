using WalletApi.Models;

namespace WalletApi.Helper
{
    public class DummyDatabaseHelper
    {
        private DummyDatabase _database {  get; set; }

        public DummyDatabaseHelper(DummyDatabase database)
        {
            _database= database;
        }

        public Player GetUserByFullUniqueName(string uniqueId)
        {
            var user = _database.players.Where(x => x.fullUniqueName.Equals(uniqueId)).FirstOrDefault();
            if (user != null)
            {
                return user;
            }
            throw new Exception($"The user with {uniqueId} was not found");
        }

        public void AddUser(Player player)
        {
            _database.players.Add(player);
        }
    }
}
