using System;

namespace HouseholdManagementAPI.Models.Domain
{
    public class Invite
    {
        public string Id { get; set; }

        public string InviteeId { get; set; }
        public virtual ApplicationUser Invitee { get; set; }

        public string InvitedId { get; set; }
        public virtual ApplicationUser Invited { get; set; }

        public string HouseholdId { get; set; }
        public virtual Household Household { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public Invite()
        {
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
            ExpiryDate = CreatedDate.AddHours(12);
        }

        public bool IsInvitationValid(DateTime givendatetime)
        {
            var result = givendatetime.CompareTo(ExpiryDate);
            
            if(result == 0 || result > 0)
            {
                return false;
            }

            return true;
        }
    }
}