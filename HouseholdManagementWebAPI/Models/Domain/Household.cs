using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdManagementWebAPI.Models.Domain
{
    public class Household
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public virtual ApplicationUser HouseholdOwner { get; set; }
        public string HouseholdOwnerId { get; set; }

        public virtual List<ApplicationUser> HouseholdMembers { get; set; }

        public virtual List<Invite> ActiveInvites { get; set; }

        public virtual List<Category> Categories { get; set; }

        public virtual List<BankAccount> BankAccounts { get; set; }

        public Household()
        {
            Id = Guid.NewGuid().ToString();
            HouseholdMembers = new List<ApplicationUser>();
            ActiveInvites = new List<Invite>();
            Categories = new List<Category>();
            BankAccounts = new List<BankAccount>();
            DateCreated = DateTime.Now;
        }
    }
}