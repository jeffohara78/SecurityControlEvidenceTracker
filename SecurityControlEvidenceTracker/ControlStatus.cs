namespace SecurityControlEvidenceTracker.Models;

public enum ControlStatus
{
    NotStarted = 1,
    InProgress = 2,
    NeedsEvidence = 3,
    ReadyForReview = 4,
    Complete = 5
}