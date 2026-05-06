namespace LeaveFlowAPI.Models
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Employee = "Employee";
    }

    public static class FormStatus
    {
        public const string Draft = "Draft";
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Cancelled = "Cancelled";
    }

    public static class FormType
    {
        public const string Leave = "Leave";
        public const string BusinessTrip = "BusinessTrip";
    }

    public static class ApprovalAction
    {
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }
}