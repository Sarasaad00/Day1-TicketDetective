using Day1_TicketDetective.Data;
using Day1_TicketDetective.Models;
using Day1_TicketDetective.Services;
using Day1_TicketDetective.Solutions;
using Day1_TicketDetective.UI;

// ============================================================================
// TRAINER SWITCH: after discussing a ticket's bug with the group, flip this
// to true and re-run the same scenario to show the corrected behavior live.
// ============================================================================
bool useFixedVersion = false;

var db = ClinicDatabase.SeedSampleData();

IAppointmentService appointmentService = useFixedVersion
    ? new AppointmentServiceFixed(db)
    : new AppointmentService(db);

IPatientService patientService = useFixedVersion
    ? new PatientServiceFixed(db)
    : new PatientService(db);

var doctor1 = db.Doctors[0]; // Dr. Samir Fathy, 9-17
var doctor2 = db.Doctors[1]; // Dr. Laila Hassan, 10-18
var patient1 = db.Patients[0]; // Mona Ali
var patient2 = db.Patients[1]; // Ahmed Kader
var tomorrow = DateTime.Today.AddDays(1);

bool running = true;
while (running)
{
    Console.Clear();
    ConsoleUI.Header("Day 1 - Ticket Detective");
    Console.WriteLine($"  Mode: {(useFixedVersion ? "FIXED (after)" : "ORIGINAL (before)")}\n");
    Console.WriteLine("  Pick a ticket scenario to run:\n");
    Console.WriteLine("  1) Ticket #1 - Book Appointment");
    Console.WriteLine("  2) Ticket #2 - Cancel Appointment");
    Console.WriteLine("  3) Ticket #3 - Available Slots");
    Console.WriteLine("  4) Ticket #4 - Register Patient");
    Console.WriteLine("  5) Ticket #5 - Doctor's Daily Schedule");
    Console.WriteLine("  0) Exit");
    Console.Write("\n  Your choice: ");

    var choice = Console.ReadLine();
    Console.Clear();

    switch (choice)
    {
        case "1": RunTicket1(); break;
        case "2": RunTicket2(); break;
        case "3": RunTicket3(); break;
        case "4": RunTicket4(); break;
        case "5": RunTicket5(); break;
        case "0": running = false; break;
        default:
            ConsoleUI.Error("Invalid choice.");
            break;
    }

    if (running)
        ConsoleUI.Pause();
}

// ------------------------------------------------------------------
// Every scenario below only DESCRIBES what happened - it never assumes
// whether the current implementation is the buggy one or the fixed one.
// That judgment is the discussion point for the room.
// ------------------------------------------------------------------

void PrintOutcome(bool isSuccess, string message)
{
    if (isSuccess) ConsoleUI.Success(message);
    else ConsoleUI.Error(message);
}

void RunTicket1()
{
    ConsoleUI.Header("Ticket #1 - Book Appointment");
    ConsoleUI.Info($"Existing appointment: Dr. {doctor1.FullName} is booked tomorrow at {tomorrow.AddHours(10):HH:mm} (30 min).");

    ConsoleUI.ScenarioStep("Try to book the SAME doctor tomorrow at 10:15 (overlaps the 10:00-10:30 slot).");
    var result1 = appointmentService.BookAppointment(patient2.Id, doctor1.Id, tomorrow.AddHours(10).AddMinutes(15));
    PrintOutcome(result1.IsSuccess, result1.IsSuccess ? $"Booked at {result1.Value!.StartTime:HH:mm}. {result1.Message}" : result1.Message);
    ConsoleUI.Info("Discuss: should this have been allowed?");

    ConsoleUI.ScenarioStep($"Try to book Dr. {doctor1.FullName} tomorrow at 20:00 (doctor works {doctor1.WorkStartHour}:00-{doctor1.WorkEndHour}:00).");
    var result2 = appointmentService.BookAppointment(patient2.Id, doctor1.Id, tomorrow.AddHours(20));
    PrintOutcome(result2.IsSuccess, result2.IsSuccess ? $"Booked at {result2.Value!.StartTime:HH:mm}. {result2.Message}" : result2.Message);
    ConsoleUI.Info("Discuss: should this have been allowed?");
}

void RunTicket2()
{
    ConsoleUI.Header("Ticket #2 - Cancel Appointment");

    var pastAppointment = db.Appointments.First(a => a.DoctorId == doctor2.Id);
    ConsoleUI.Info($"Appointment #{pastAppointment.Id} with Dr. {doctor2.FullName} was scheduled at {pastAppointment.StartTime:g} (already in the past).");

    ConsoleUI.ScenarioStep($"Cancel appointment #{pastAppointment.Id}.");
    var result1 = appointmentService.CancelAppointment(pastAppointment.Id);
    PrintOutcome(result1.IsSuccess, result1.Message);
    ConsoleUI.Info("Discuss: does cancelling an appointment that already happened make sense?");

    ConsoleUI.ScenarioStep($"Cancel appointment #{pastAppointment.Id} again.");
    var result2 = appointmentService.CancelAppointment(pastAppointment.Id);
    PrintOutcome(result2.IsSuccess, result2.Message);
    ConsoleUI.Info("Discuss: should cancelling an already-cancelled appointment succeed again?");
}

void RunTicket3()
{
    ConsoleUI.Header("Ticket #3 - Available Slots");

    ConsoleUI.Info($"Current time right now: {DateTime.Now:HH:mm}");
    ConsoleUI.ScenarioStep($"Ask for Dr. {doctor1.FullName}'s available slots for TODAY.");

    var result = appointmentService.GetAvailableSlots(doctor1.Id, DateTime.Today);
    if (result.IsSuccess)
    {
        if (result.Value!.Count == 0)
        {
            ConsoleUI.Info("No slots returned.");
        }
        foreach (var slot in result.Value!)
        {
            bool isPast = slot < DateTime.Now;
            if (isPast) ConsoleUI.Error($"{slot:HH:mm} - returned as available (this time has already passed today)");
            else ConsoleUI.Success($"{slot:HH:mm} - available");
        }
    }
    else
    {
        ConsoleUI.Error(result.Message);
    }
}

void RunTicket4()
{
    ConsoleUI.Header("Ticket #4 - Register Patient");
    ConsoleUI.Info($"Existing patient: #{patient1.Id} {patient1.FullName}, National ID: {patient1.NationalId}");

    ConsoleUI.ScenarioStep("Reception registers a patient again with the SAME National ID (maybe a typo in the name).");
    var result = patientService.RegisterPatient("Mona Aly", "01001234567", patient1.NationalId);
    PrintOutcome(result.IsSuccess, result.IsSuccess ? $"New patient ID: #{result.Value!.Id}. {result.Message}" : result.Message);

    var matches = db.Patients.Where(p => p.NationalId == patient1.NationalId).ToList();
    ConsoleUI.Info($"Patients currently in the system with National ID {patient1.NationalId}:");
    foreach (var p in matches)
        ConsoleUI.Info($"  -> Patient #{p.Id}: {p.FullName}");
}

void RunTicket5()
{
    ConsoleUI.Header("Ticket #5 - Doctor's Daily Schedule");

    ConsoleUI.ScenarioStep($"Get Dr. {doctor1.FullName}'s schedule for tomorrow ({tomorrow:yyyy-MM-dd}).");
    var result = appointmentService.GetDoctorScheduleForDay(doctor1.Id, tomorrow);

    if (result.IsSuccess)
    {
        if (result.Value!.Count == 0)
        {
            ConsoleUI.Info("No appointments returned.");
        }
        foreach (var appt in result.Value!)
        {
            var statusTag = appt.Status == AppointmentStatus.Cancelled ? "CANCELLED" : "booked";
            ConsoleUI.Info($"{appt.StartTime:HH:mm} - appointment #{appt.Id} ({statusTag})");
        }
        ConsoleUI.Info("Discuss: should cancelled appointments appear here? Is the list sorted by time?");
    }
    else
    {
        ConsoleUI.Error(result.Message);
    }
}
