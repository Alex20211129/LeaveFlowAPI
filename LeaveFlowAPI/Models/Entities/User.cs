namespace LeaveFlowAPI.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Employee"; // Employee / Manager / Admin
        public int? ManagerId { get; set; }

        // Navigation properties
        public User? Manager { get; set; }
        public ICollection<User> Subordinates { get; set; } = new List<User>();
        public ICollection<Form> Forms { get; set; } = new List<Form>();
        public ICollection<ApprovalRecord> ApprovalRecords { get; set; } = new List<ApprovalRecord>();
    }
}