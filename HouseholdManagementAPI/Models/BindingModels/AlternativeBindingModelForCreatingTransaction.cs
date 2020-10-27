using System;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManagementAPI.Models.BindingModels
{
    public class AlternativeBindingModelForCreatingTransaction
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string CategoryId { get; set; }
        [Required]
        public string BankAccountId { get; set; }
    }
}