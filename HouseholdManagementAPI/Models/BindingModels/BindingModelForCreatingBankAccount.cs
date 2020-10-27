using System.ComponentModel.DataAnnotations;

namespace HouseholdManagementAPI.Models.BindingModels
{
    public class BindingModelForCreatingBankAccount
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}