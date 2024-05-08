using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Transactions;
using WalletApi.Controllers;
using WalletApi.Helper;
using WalletApi.Models;
using WalletApiTests.Helper;

namespace WalletApiTests
{
    public class WalletApiTests
    {
        [Fact]
        public void DefaultNumberOfUsersShallbeFour()
        {
            WalletApiHandler handler = new WalletApiHandler();
            Assert.Equal(handler.GetUserIds()?.Count(), 4);
        }

        [Fact]
        public void DeleteOneUser()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var user = handler.GetUserIds().First();
            Assert.NotNull(user);
            handler.exposedGameApi.DeleteUser(user);
            Assert.Equal(handler.GetUserIds()?.Count(), 3);
        }

        [Fact]
        public void DeleteAllUsers()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var users = handler.GetUserIds().ToArray();
            for (int i = 0; i < users.Count(); i++)
            {
                Assert.NotNull(users[i]);
                handler.exposedGameApi.DeleteUser(users[i]);
            }
            Assert.Equal(handler.GetUserIds()?.Count(), 0);
        }

        [Fact]
        public void CreateNewUserDepositBalance()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var newUserId = handler.GetValueFromActionResult<string>
                (handler.exposedGameApi.NewUser("johnny", null));
            Assert.NotNull(newUserId);

            handler.exposedGameApi.CreateWallet(newUserId, 50, false);
            var balance = handler.GetValueFromActionResult<decimal>
                (handler.exposedGameApi.GetUserBalance(newUserId));

            Assert.Equal(50, balance);
        }

        [Fact]
        public void CreateNewUserDepositNegativeBalance()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var newUserId = handler.GetValueFromActionResult<string>
                (handler.exposedGameApi.NewUser("johnny", null));
            Assert.NotNull(newUserId);

            handler.exposedGameApi.CreateWallet(newUserId, -50, false);
            var balance = handler.GetValueFromActionResult<decimal>
                (handler.exposedGameApi.GetUserBalance(newUserId));

            Assert.Equal(0, balance);
        }

        [Fact]
        public void RewriteExistingWallet()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var user = handler.GetUserIds().First();

            handler.exposedGameApi.CreateWallet(user, 500, true);
            var balance = handler.GetValueFromActionResult<decimal>
                (handler.exposedGameApi.GetUserBalance(user));

            Assert.Equal(500, balance);
        }

        [Fact]
        public void RewriteExistingWalletWithNegativeValue()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var user = handler.GetUserIds().First();

            handler.exposedGameApi.CreateWallet(user, -54989219, true);
            var balance = handler.GetValueFromActionResult<decimal>
                (handler.exposedGameApi.GetUserBalance(user));

            Assert.Equal(0, balance);
        }

        [Fact]
        public void StakeMoneyTest()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var user = handler.GetUserIds().First();

            var balanceOld = handler.GetValueFromActionResult<decimal>
                (handler.exposedGameApi.GetUserBalance(user));

            handler.exposedGameApi.UpdateUserBalanceSubstract(user, (balanceOld / 2) * -1);

            var balanceNew = handler.GetValueFromActionResult<decimal>
                    (handler.exposedGameApi.GetUserBalance(user));

            Assert.Equal((balanceNew * 2), balanceOld);
        }


        [Fact]
        public void StakeMoneyTestLargeSum()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var user = handler.GetUserIds().First();

            var balanceOld = handler.GetValueFromActionResult<decimal>
                (handler.exposedGameApi.GetUserBalance(user));
            try
            {
                handler.exposedGameApi.UpdateUserBalanceSubstract
                     (user, decimal.MaxValue * -1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var balanceNew = handler.GetValueFromActionResult<decimal>
                    (handler.exposedGameApi.GetUserBalance(user));

            Assert.Equal(0, balanceNew);
        }

        [Fact]
        public async Task StakeMoneyInParallelToZero()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var user = handler.GetUserIds().First();

            handler.exposedGameApi.CreateWallet(user, 5000, true);
            var tasks = new List<Task>();

            for (int i = 0; i < 1000; i++)
            {
                Task task = Task.Run(() =>
                {
                    handler.exposedGameApi.UpdateUserBalanceSubstract(user, -5);
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            var balanceNew = handler.GetValueFromActionResult<decimal>
                        (handler.exposedGameApi.GetUserBalance(user));

            Assert.Equal(0, balanceNew);
        }

        [Fact]
        public async Task StakeMoneyInParallel()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var user = handler.GetUserIds().First();

            handler.exposedGameApi.CreateWallet(user, 5000, true);
            var tasks = new List<Task>();

            for (int i = 0; i < 952; i++)
            {
                Task task = Task.Run(() =>
                {
                    handler.exposedGameApi.UpdateUserBalanceSubstract(user, -5);
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            var balanceNew = handler.GetValueFromActionResult<decimal>
                        (handler.exposedGameApi.GetUserBalance(user));

            Assert.Equal(240, balanceNew);
        }

        [Fact]
        public void DepositMoney()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var user = handler.GetUserIds().First();
            handler.exposedGameApi.CreateWallet(user, 0, true);

            for (int i = 0; i < 1000; i++)
            {
                handler.exposedGameApi.UpdateUserBalanceAdd(user, 5);
            }

            var balanceNew = handler.GetValueFromActionResult<decimal>
                        (handler.exposedGameApi.GetUserBalance(user));
            Assert.Equal(5000, balanceNew);
        }

        [Fact]
        public async Task DepositMoneyInParallel()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var user = handler.GetUserIds().First();
            handler.exposedGameApi.CreateWallet(user, 0, true);
            var tasks = new List<Task>();

            for (int i = 0; i < 1000; i++)
            {
                Task task = Task.Run(() =>
                {
                    handler.exposedGameApi.UpdateUserBalanceAdd(user, 5);
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            var balanceNew = handler.GetValueFromActionResult<decimal>
                        (handler.exposedGameApi.GetUserBalance(user));

            Assert.Equal(5000, balanceNew);
        }


        [Fact]
        public void TransferMoneyFromOneUserToAnother()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var userFrom = handler.GetUserIds().First();
            var userTo = handler.GetUserIds().Last();

            handler.exposedGameApi.CreateWallet(userFrom, 5000, true);
            handler.exposedGameApi.CreateWallet(userTo, 5000, true);

            for (int i = 0; i < 1000; i++)
            {
                handler.exposedGameApi.TransferMoney(userFrom, userTo, 5);
            }

            var balanceFromUser = handler.GetValueFromActionResult<decimal>
            (handler.exposedGameApi.GetUserBalance(userFrom));

            var balanceToUser = handler.GetValueFromActionResult<decimal>
                (handler.exposedGameApi.GetUserBalance(userTo));

            Assert.Equal(10000, balanceToUser);
            Assert.Equal(0, balanceFromUser);
        }

        [Fact]
        public async Task TransferMoneyFromOneUserToAnotherParallel()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var userFrom = handler.GetUserIds().First();
            var userTo = handler.GetUserIds().Last();

            handler.exposedGameApi.CreateWallet(userFrom, 5000, true);
            handler.exposedGameApi.CreateWallet(userTo, 5000, true);

            var tasks = new List<Task>();
            for (int i = 0; i < 1000; i++)
            {
                Task task = Task.Run(() =>
                {
                    handler.exposedGameApi.TransferMoney(userFrom, userTo, 5);
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
            var balanceFromUser = handler.GetValueFromActionResult<decimal>
            (handler.exposedGameApi.GetUserBalance(userFrom));

            var balanceToUser = handler.GetValueFromActionResult<decimal>
                (handler.exposedGameApi.GetUserBalance(userTo));

            Assert.Equal(10000, balanceToUser);
            Assert.Equal(0, balanceFromUser);
        }

        [Fact]
        public async Task CheckTransactionsDoneInParallel()
        {
            WalletApiHandler handler = new WalletApiHandler();
            var user = handler.GetUserIds().First();
            handler.exposedGameApi.CreateWallet(user, 0, true);

            Random random = new Random();
            decimal minValue = 0.0m;
            decimal maxValue = 100.0m;
            var tasks = new List<Task>();

            for (int i = 0; i < 1000; ++i)
            {
                Task task = Task.Run(() =>
                {
                    decimal randomValue = (decimal)(random.NextDouble() * 
                    (double)(maxValue - minValue) + (double)minValue);
                    handler.exposedGameApi.UpdateUserBalanceAdd(user, randomValue);
                });
                tasks.Add(task);
            }

            for (int i = 0; i < 1000; ++i)
            {
                Task task = Task.Run(() =>
                {
                    decimal randomValue = (decimal)(random.NextDouble() * 
                    (double)(maxValue - minValue) + (double)minValue);
                    handler.exposedGameApi.UpdateUserBalanceSubstract(user, randomValue);
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            var balance = handler.GetValueFromActionResult<decimal>
                    (handler.exposedGameApi.GetUserBalance(user));

            var transactions=handler.GetValueFromActionResult<Transactions>
                (handler.exposedGameApi.GetTransactions(user));

            var summupOfTransactions=transactions?.transactions?.Select(x=>x.transactionValue).Sum();

            Assert.Equal(summupOfTransactions, balance);
        }
    }
}