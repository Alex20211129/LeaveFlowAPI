namespace LeaveFlowAPI.DTOs
{
    public class ApprovalActionDto
    {
        public string Comment { get; set; } = string.Empty;
    }

    public class ApprovalResponseDto
    {
        public int FormId { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ApprovalRecordResponseDto
    {
        public int Id { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime ActionAt { get; set; }
    }
}