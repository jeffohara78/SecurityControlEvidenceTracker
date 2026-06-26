namespace SecurityControlEvidenceTracker.Models;

public class SecurityControl
{
    public int Id { get; set; }

    public string ControlName { get; set; } = "";

    public string Framework { get; set; } = "";

    public string ControlCategory { get; set; } = "";

    public string Owner { get; set; } = "";

    public string EvidenceDescription { get; set; } = "";

    public DateTime DueDate { get; set; }

    public ControlStatus Status { get; set; }

    public string Notes { get; set; } = "";
}