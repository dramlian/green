using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletApi.Controllers;
using WalletApi.Models;

namespace WalletApiTests.Helper
{
    internal class WalletApiHandler
    {
        private DummyDatabase _playerHandler {  get; set; }
        public ExposedGameApi exposedGameApi { get; set; }

        public WalletApiHandler()
        {
            _playerHandler = new DummyDatabase();
            exposedGameApi = new ExposedGameApi(_playerHandler);
        }

        public IEnumerable<string> GetUserIds()
        {
            var actionResult = exposedGameApi.GetUsers();
            var okResult = actionResult as OkObjectResult;
            return okResult?.Value as IEnumerable<string> ?? Enumerable.Empty<string>();
        }

        public T? GetValueFromActionResult<T>(IActionResult actionResult)
        {
            if (actionResult is OkObjectResult okResult && okResult.Value is T resultValue)
            {
                return resultValue;
            }

            return default(T);
        }



    }
}
