using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdManagementWebAPI.Models.BindingModels
{
    public class InviteBindingModel
    {
        public string Id { get; set; }
        //public string Email { get; set; }
        public string HouseholdId { get; set; }
        public string InviteeId { get; set; }
        public string InvitedId { get; set; }
        //public string InviteeUsername { get; set; }        
    }
}