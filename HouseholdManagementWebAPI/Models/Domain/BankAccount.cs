using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdManagementWebAPI.Models.Domain
{
    public class BankAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public decimal Balance { get; set; }

        public string HouseholdId { get; set; }
        public virtual Household Household { get; set; }

        public virtual List<Transaction> Transactions { get; set; }

        public BankAccount()
        {
            Id = Guid.NewGuid().ToString();
            Balance = 0;
            Transactions = new List<Transaction>();
            DateCreated = DateTime.Now;
        }        
    }
}