using SecurityControlEvidenceTracker.Models;
using SecurityControlEvidenceTracker.Services;

namespace SecurityControlEvidenceTracker.UI;

public class ConsoleUI
{
    private readonly ControlManager _controlManager = new();

    public void Run()
    {
        bool running = true;

        while (running)
        {
            Console.Clear();
            ShowHeader();

            Console.WriteLine("1. Add security control");
            Console.WriteLine("2. View all controls");
            Console.WriteLine("3. Add or update evidence");
            Console.WriteLine("4. Update control status");
            Console.WriteLine("5. View audit readiness dashboard");
            Console.WriteLine("6. Delete control");
            Console.WriteLine("7. Exit");

            Console.Write("\nChoose an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddControl();
                    break;
                case "2":
                    ViewAllControls();
                    break;
                case "3":
                    AddEvidence();
                    break;
                case "4":
                    UpdateStatus();
                    break;
                case "5":
                    ViewDashboard();
                    break;
                case "6":
                    DeleteControl();
                    break;
                case "7":
                    running = false;
                    break;
                default:
                    Pause("Invalid choice.");
                    break;
            }
        }
    }

    private void ShowHeader()
    {
        Console.WriteLine("==========================================");
        Console.WriteLine("   SECURITY CONTROL EVIDENCE TRACKER");
        Console.WriteLine("==========================================\n");
    }

    private void AddControl()
    {
        Console.Clear();
        ShowHeader();

        Console.WriteLine("Add New Security Control");
        Console.WriteLine("------------------------------------------");
        Console.WriteLine("Enter 0 at any time to cancel and return to the main menu.\n");

        SecurityControl control = new();

        Console.WriteLine("CONTROL NAME");
        Console.WriteLine("Example: Multi-Factor Authentication Required");
        string? controlName = ReadInputOrCancel("Control name: ");
        if (controlName == null) return;
        control.ControlName = controlName;

        Console.WriteLine();

        Console.WriteLine("FRAMEWORK");
        Console.WriteLine("Examples: NIST, SOC 2, ISO 27001, HIPAA, PCI-DSS");
        string? framework = ReadInputOrCancel("Framework: ");
        if (framework == null) return;
        control.Framework = framework;

        Console.WriteLine();

        Console.WriteLine("CONTROL CATEGORY");
        Console.WriteLine("Examples: Access Control, Logging and Monitoring, Incident Response");
        string? category = ReadInputOrCancel("Control category: ");
        if (category == null) return;
        control.ControlCategory = category;

        Console.WriteLine();

        Console.WriteLine("CONTROL OWNER");
        Console.WriteLine("Examples: IT Manager, Security Team, Help Desk");
        string? owner = ReadInputOrCancel("Control owner: ");
        if (owner == null) return;
        control.Owner = owner;

        Console.WriteLine();

        Console.WriteLine("EVIDENCE DESCRIPTION");
        Console.WriteLine("Examples: Screenshot of MFA policy, access review spreadsheet");
        Console.WriteLine("Leave blank if evidence has not been collected yet.");
        string? evidence = ReadInputOrCancel("Evidence description: ");
        if (evidence == null) return;
        control.EvidenceDescription = evidence;

        Console.WriteLine();

        Console.WriteLine("DUE DATE");
        Console.WriteLine("Example: 07/15/2026");
        DateTime? dueDate = ReadDateOrCancel("Due date: ");
        if (dueDate == null) return;
        control.DueDate = dueDate.Value;

        control.Status = string.IsNullOrWhiteSpace(control.EvidenceDescription)
            ? ControlStatus.NeedsEvidence
            : ControlStatus.ReadyForReview;

        Console.WriteLine();

        Console.WriteLine("NOTES");
        Console.WriteLine("Example: Waiting on IT to provide screenshot");
        string? notes = ReadInputOrCancel("Notes: ");
        if (notes == null) return;
        control.Notes = notes;

        _controlManager.AddControl(control);

        Pause($"Security control added successfully. Initial status: {control.Status}");
    }

    private void ViewAllControls()
    {
        Console.Clear();
        ShowHeader();

        List<SecurityControl> controls = _controlManager.GetAllControls();

        if (!controls.Any())
        {
            Pause("No controls have been added yet.");
            return;
        }

        foreach (SecurityControl control in controls)
        {
            DisplayControl(control);
        }

        Pause();
    }

    private void AddEvidence()
    {
        Console.Clear();
        ShowHeader();

        DisplayControlSummary();

        int id = ReadInt("\nEnter control ID to update evidence: ");

        Console.Write("Enter evidence description: ");
        string evidence = Console.ReadLine() ?? "";

        bool updated = _controlManager.AddEvidence(id, evidence);

        Pause(updated ? "Evidence updated successfully." : "Control not found.");
    }

    private void UpdateStatus()
    {
        Console.Clear();
        ShowHeader();

        DisplayControlSummary();

        int id = ReadInt("\nEnter control ID to update status: ");

        Console.WriteLine("\nChoose new status:");
        Console.WriteLine("1. Not Started");
        Console.WriteLine("2. In Progress");
        Console.WriteLine("3. Needs Evidence");
        Console.WriteLine("4. Ready For Review");
        Console.WriteLine("5. Complete");

        int statusChoice = ReadInt("\nStatus choice: ");

        if (statusChoice < 1 || statusChoice > 5)
        {
            Pause("Invalid status choice.");
            return;
        }

        ControlStatus newStatus = (ControlStatus)statusChoice;

        bool updated = _controlManager.UpdateStatus(id, newStatus);

        Pause(updated ? "Status updated successfully." : "Control not found.");
    }

    private void ViewDashboard()
    {
        Console.Clear();
        ShowHeader();

        List<SecurityControl> controls = _controlManager.GetAllControls();

        if (!controls.Any())
        {
            Pause("No controls have been added yet.");
            return;
        }

        int total = controls.Count;
        int complete = controls.Count(c => c.Status == ControlStatus.Complete);
        int readyForReview = controls.Count(c => c.Status == ControlStatus.ReadyForReview);
        int needsEvidence = controls.Count(c => c.Status == ControlStatus.NeedsEvidence);
        int overdue = _controlManager.GetOverdueControls().Count;

        double completionPercentage = total == 0 ? 0 : Math.Round((complete / (double)total) * 100, 2);

        Console.WriteLine("Audit Readiness Dashboard");
        Console.WriteLine("------------------------------------------");
        Console.WriteLine($"Total Controls: {total}");
        Console.WriteLine($"Complete Controls: {complete}");
        Console.WriteLine($"Ready For Review: {readyForReview}");
        Console.WriteLine($"Needs Evidence: {needsEvidence}");
        Console.WriteLine($"Overdue Controls: {overdue}");
        Console.WriteLine($"Completion Percentage: {completionPercentage}%");

        Console.WriteLine("\nControls Missing Evidence:");
        List<SecurityControl> missingEvidence = _controlManager.GetControlsMissingEvidence();

        if (!missingEvidence.Any())
        {
            Console.WriteLine("- None");
        }
        else
        {
            foreach (SecurityControl control in missingEvidence)
            {
                Console.WriteLine($"- ID {control.Id}: {control.ControlName}");
            }
        }

        Console.WriteLine("\nOverdue Controls:");
        List<SecurityControl> overdueControls = _controlManager.GetOverdueControls();

        if (!overdueControls.Any())
        {
            Console.WriteLine("- None");
        }
        else
        {
            foreach (SecurityControl control in overdueControls)
            {
                Console.WriteLine($"- ID {control.Id}: {control.ControlName} | Due: {control.DueDate.ToShortDateString()}");
            }
        }

        Pause();
    }

    private void DeleteControl()
    {
        Console.Clear();
        ShowHeader();

        DisplayControlSummary();

        int id = ReadInt("\nEnter control ID to delete: ");

        bool deleted = _controlManager.DeleteControl(id);

        Pause(deleted ? "Control deleted successfully." : "Control not found.");
    }

    private void DisplayControl(SecurityControl control)
    {
        Console.WriteLine($"ID: {control.Id}");
        Console.WriteLine($"Control: {control.ControlName}");
        Console.WriteLine($"Framework: {control.Framework}");
        Console.WriteLine($"Category: {control.ControlCategory}");
        Console.WriteLine($"Owner: {control.Owner}");
        Console.WriteLine($"Status: {control.Status}");
        Console.WriteLine($"Due Date: {control.DueDate.ToShortDateString()}");
        Console.WriteLine($"Evidence: {control.EvidenceDescription}");
        Console.WriteLine($"Notes: {control.Notes}");
        Console.WriteLine("------------------------------------------");
    }

    private void DisplayControlSummary()
    {
        List<SecurityControl> controls = _controlManager.GetAllControls();

        if (!controls.Any())
        {
            Console.WriteLine("No controls have been added yet.");
            return;
        }

        foreach (SecurityControl control in controls)
        {
            Console.WriteLine($"{control.Id}. {control.ControlName} | {control.Framework} | {control.Status}");
        }
    }

    private int ReadInt(string prompt)
    {
        int number;

        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out number))
            {
                return number;
            }

            Console.WriteLine("Please enter a valid number.");
        }
    }

    private DateTime ReadDate(string prompt)
    {
        DateTime date;

        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (DateTime.TryParse(input, out date))
            {
                return date;
            }

            Console.WriteLine("Please enter a valid date.");
        }
    }

    private void Pause(string message = "Press Enter to continue.")
    {
        Console.WriteLine($"\n{message}");
        Console.ReadLine();
    }

    private string? ReadInputOrCancel(string prompt)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (input == "0")
        {
            return null;
        }

        return input ?? "";
    }
    private DateTime? ReadDateOrCancel(string prompt)
    {
        DateTime date;

        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (input == "0")
            {
                return null;
            }

            if (DateTime.TryParse(input, out date))
            {
                return date;
            }

            Console.WriteLine("Please enter a valid date or enter 0 to cancel.");
        }
    }
}