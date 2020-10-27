using System;

namespace HouseholdManagementAPI.Models.BindingModels
{
    public class CategoryBindingModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; } 
        public string HouseholdId { get; set; }
        public string HouseholdName { get; set; }
    }
}