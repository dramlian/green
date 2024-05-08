using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WalletApi.Models
{
    public class Wallet
    {
        public decimal balance { get; set; }

        public string ownerName { get; set; }

        public List<Transaction> transactionHistory { get; set; }

        public Wallet(string fullOwnerName,decimal initialBalance=0)
        {
            ownerName = fullOwnerName;
            balance = initialBalance;
            transactionHistory = new List<Transaction>();
        }


    }
}
