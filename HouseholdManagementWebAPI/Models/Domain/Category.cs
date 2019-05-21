using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdManagementWebAPI.Models.Domain
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public string HouseholdId { get; set; }
        public virtual Household Household { get; set; }

        public Category()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}