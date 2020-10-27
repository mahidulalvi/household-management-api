namespace HouseholdManagementAPI.Models.BindingModels
{
    public class InviteBindingModel
    {
        public string Id { get; set; }
        public string HouseholdId { get; set; }
        public string InviteeId { get; set; }
        public string InvitedId { get; set; }
    }
}