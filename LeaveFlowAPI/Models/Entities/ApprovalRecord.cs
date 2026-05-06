namespace LeaveFlowAPI.Models.Entities
{
    public class ApprovalRecord
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public int ManagerId { get; set; }
        public string Action { get; set; } = string.Empty; // Approved / Rejected
        public string Comment { get; set; } = string.Empty;
        public DateTime ActionAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Form Form { get; set; } = null!;
        public User Manager { get; set; } = null!;
    }
}