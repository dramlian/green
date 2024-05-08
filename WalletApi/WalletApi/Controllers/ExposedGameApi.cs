using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WalletApi.Helper;
using WalletApi.Models;

namespace WalletApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExposedGameApi : ControllerBase
    {
        public DummyDatabase _playersModel { get; set; }

        private DummyDatabaseHelper _playersHandler {  get; set; }
        public ExposedGameApi(DummyDatabase playerHandler) { 
            _playersModel = playerHandler;
            _playersHandler = new DummyDatabaseHelper(_playersModel);
        }

        /// <summary>
        /// Gets a list of unique ids of all available users.
        /// Use those ids to interact with the given users in other
        /// endpoints.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/getUsers")]
        public IActionResult GetUsers()
        {
            var users = _playersModel.players.Select
                (x => x.fullUniqueName);

            return Ok(users);
        }

        /// <summary>
        /// Deletes a specific user based on their ID.
        /// </summary>
        /// <param name="id">Unique id, exposed in getUsers call</param>
        /// <returns></returns>
        [HttpDelete("/deleteUser/{id}")]
        public IActionResult DeleteUser(string id)
        {
            var user=_playersModel.players.Where(x=>x.fullUniqueName
            .Equals(id)).FirstOrDefault();

            if (user == null)
            {
                return BadRequest("The user does not exist");
            }

            _playersModel.players.Remove(user);
            return Ok(GetUsers());
        }

        /// <summary>
        /// Adds funds to a given user.
        /// </summary>
        /// <param name="id">Unique id, exposed in getUsers call</param>
        /// <param name="newBalanceVal">Can not be a negative ammount</param>
        /// <returns></returns>
        [HttpPut("/balancedeposit/{id}")]
        [HttpPut("/balancewin/{id}")]
        public IActionResult UpdateUserBalanceAdd(string id, [FromBody] decimal newBalanceVal)
        {
            if (newBalanceVal < 0)
            {
                return BadRequest("You can not deposit a negative ammount");
            }
            var user = _playersHandler.GetUserByFullUniqueName(id);
            var userHelper=new PlayerHelper(user);
            lock (user)
            {
                userHelper.EditUserBalance(newBalanceVal);
                return Ok($"User balance updated successfully, new balance for {id} is {userHelper.GetUserBalance()} ");
            }
        }

        /// <summary>
        /// Substracts funds from a given user
        /// </summary>
        /// <param name="id"> Unique id, exposed in getUsers call </param>
        /// <param name="newBalanceVal"> Can not be a positive ammount</param>
        /// <returns></returns>
        [HttpPut("/stake/{id}")]
        public IActionResult UpdateUserBalanceSubstract(string id, [FromBody] decimal newBalanceVal) //some middlware web api error handling
        {
            if (newBalanceVal > 0)
            {
                return BadRequest("You can not stake a positive ammount");
            }
            var user = _playersHandler.GetUserByFullUniqueName(id); //DRY
            var userHelper = new PlayerHelper(user);
            lock (user)
            {
                userHelper.EditUserBalance(newBalanceVal);
                return Ok($"User balance updated successfully, new balance for {id} is {userHelper.GetUserBalance()} ");
            }
        }

        /// <summary>
        /// Gets a balance of a given user
        /// </summary>
        /// <param name="id"> Unique id, exposed in getUsers call </param>
        /// <returns></returns>
        [HttpGet("/balance/{id}")]
        public IActionResult GetUserBalance(string id)
        {
            var user = _playersHandler.GetUserByFullUniqueName(id);
            var userHelper = new PlayerHelper(user);
            return Ok(userHelper.GetUserBalance());
        }

        /// <summary>
        /// Gets transactions of a given user
        /// </summary>
        /// <param name="id"> Unique id, exposed in getUsers call</param>
        /// <returns></returns>
        [HttpGet("/transactions/{id}")]
        [Produces("application/json")]
        public IActionResult GetTransactions(string id)
        {
            var user = _playersHandler.GetUserByFullUniqueName(id);
            var userHelper = new PlayerHelper(user);
            return Ok(userHelper.GetUserTrasnactions());
        }

        /// <summary>
        /// Creates a new wallet for a user. Every user can only have one wallet.
        /// </summary>
        /// <param name="id"> Unique id, exposed in getUsers call </param>
        /// <param name="balance"> Balance to be set</param>
        /// <param name="forceToDel">If false and if the user has a wallet registered, badrequest is returned.
        /// If the value is set to true, current wallet is rewritten by newly created one with the provided balance</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/registerWallet/{id}")]
        public IActionResult CreateWallet(string id,decimal balance, bool forceToDel)
        {
            var user = _playersHandler.GetUserByFullUniqueName(id);
            if (user.wallet != null && !forceToDel)
            {
                return BadRequest("User has already a wallet that is registered!");
            }
            user.wallet = new Wallet(id, balance);
            var walletHelper = new WalletHelper(user.wallet);
            walletHelper.ValidateBalance();
            walletHelper.AddTransactionToHistory(balance);
            return Ok("Wallet created successfully");
        }

        /// <summary>
        /// Transfers funds from one user to another.
        /// </summary>
        /// <param name="idfrom">Unique id, exposed in getUsers call </param>
        /// <param name="idTo">The same user id but for recipient user</param>
        /// <param name="amount">Can not be less or equal to zero</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/transferFrom/{idfrom}/transferTo/{idTo}")]
        public IActionResult TransferMoney(string idfrom,string idTo, decimal amount)
        {
            if (amount <= 0)
            {
                return BadRequest("Please provided a value greater than zero");
            }

            var fromUser= _playersHandler.GetUserByFullUniqueName(idfrom);
            var userHelper = new PlayerHelper(fromUser);
            if (amount > userHelper.GetUserBalance())
            {
                return BadRequest($"The user {idfrom} does not have sufficient funds!");
            }

            UpdateUserBalanceSubstract(idfrom, amount*-1);
            UpdateUserBalanceAdd(idTo,amount);

            return Ok("Success");
        }


        /// <summary>
        /// Creates a new user with name and balance
        /// </summary>
        /// <param name="name">Unique identifier is built from this value combined with Guid </param>
        /// <param name="balance">If null then the user has no wallet registered</param>
        /// <returns>GUID unique indentificator of a user</returns>
        [HttpPost]
        [Route("/createNewUser")]
        public IActionResult NewUser(string name,decimal? balance)
        {
            Player newPlayer;

            if (balance == null){newPlayer=new Player(name);}
            else{newPlayer = new Player(name, (decimal)balance);}
            _playersHandler.AddUser(newPlayer);

            return Ok(newPlayer.fullUniqueName);
        }
    }
}
