using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdManagementWebAPI.Models.Domain
{
    public class Transaction
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool IsTransactionVoid { get; set; }

        public string CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public string BankAccountId { get; set; }
        public virtual BankAccount BankAccount { get; set; }

        public string TransactionOwnerId { get; set; }
        public virtual ApplicationUser TransactionOwner { get; set; }

        public Transaction()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
        }        
    }
}