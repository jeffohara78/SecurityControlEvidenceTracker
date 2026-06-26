using SecurityControlEvidenceTracker.Models;

namespace SecurityControlEvidenceTracker.Services;

public class ControlManager
{
    private readonly FileStorage _storage = new();
    private readonly List<SecurityControl> _controls;

    public ControlManager()
    {
        _controls = _storage.LoadControls();
    }

    public List<SecurityControl> GetAllControls()
    {
        return _controls;
    }

    public void AddControl(SecurityControl control)
    {
        control.Id = _controls.Count == 0 ? 1 : _controls.Max(c => c.Id) + 1;

        _controls.Add(control);
        _storage.SaveControls(_controls);
    }

    public SecurityControl? GetControlById(int id)
    {
        return _controls.FirstOrDefault(c => c.Id == id);
    }

    public bool UpdateStatus(int id, ControlStatus newStatus)
    {
        SecurityControl? control = GetControlById(id);

        if (control == null)
        {
            return false;
        }

        control.Status = newStatus;
        _storage.SaveControls(_controls);

        return true;
    }

    public bool AddEvidence(int id, string evidenceDescription)
    {
        SecurityControl? control = GetControlById(id);

        if (control == null)
        {
            return false;
        }

        control.EvidenceDescription = evidenceDescription;

        if (string.IsNullOrWhiteSpace(evidenceDescription))
        {
            control.Status = ControlStatus.NeedsEvidence;
        }
        else if (control.Status == ControlStatus.NeedsEvidence || control.Status == ControlStatus.InProgress)
        {
            control.Status = ControlStatus.ReadyForReview;
        }

        _storage.SaveControls(_controls);

        return true;
    }

    public bool DeleteControl(int id)
    {
        SecurityControl? control = GetControlById(id);

        if (control == null)
        {
            return false;
        }

        _controls.Remove(control);
        _storage.SaveControls(_controls);

        return true;
    }

    public List<SecurityControl> GetOverdueControls()
    {
        return _controls
            .Where(c => c.DueDate.Date < DateTime.Today && c.Status != ControlStatus.Complete)
            .ToList();
    }

    public List<SecurityControl> GetControlsMissingEvidence()
    {
        return _controls
            .Where(c => string.IsNullOrWhiteSpace(c.EvidenceDescription))
            .ToList();
    }
}