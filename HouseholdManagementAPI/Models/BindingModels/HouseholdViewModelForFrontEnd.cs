using System;
using System.Collections.Generic;

namespace HouseholdManagementAPI.Models.BindingModels
{
    public class HouseholdViewModelForFrontEnd
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public HouseholdMember HouseholdOwner { get; set; }

        public List<HouseholdMember> HouseholdMembers { get; set; }

        public HouseholdViewModelForFrontEnd()
        {
            HouseholdMembers = new List<HouseholdMember>();            
        }
    }
}