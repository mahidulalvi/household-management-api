using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using HouseholdManagementAPI.Models.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HouseholdManagementAPI.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [InverseProperty(nameof(Household.HouseholdMembers))]
        public virtual List<Household> JoinedHouseholds { get; set; }

        //nameof() can be used
        [InverseProperty(nameof(Household.HouseholdOwner))]
        public virtual List<Household> OwnedHouseholds { get; set; }

        [InverseProperty(nameof(Invite.Invitee))]
        public virtual List<Invite> SentHouseholdInvites { get; set; }

        [InverseProperty(nameof(Invite.Invited))]
        public virtual List<Invite> ReceivedHouseholdInvites { get; set; }

        //public virtual List<BankAccount> BankAccounts { get; set; }

        public virtual List<Transaction> Transactions { get; set; }

        public ApplicationUser()
        {
            JoinedHouseholds = new List<Household>();
            OwnedHouseholds = new List<Household>();
            SentHouseholdInvites = new List<Invite>();
            ReceivedHouseholdInvites = new List<Invite>();
            Transactions = new List<Transaction>();
        }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Household> Households { get; set; }

        public DbSet<Invite> Invites { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<BankAccount> BankAccounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }        

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}