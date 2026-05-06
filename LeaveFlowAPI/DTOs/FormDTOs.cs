namespace LeaveFlowAPI.DTOs
{
    public class CreateFormDto
    {
        public string Type { get; set; } = string.Empty; // Leave / BusinessTrip
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class FormResponseDto
    {
        public int Id { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}