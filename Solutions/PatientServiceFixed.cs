using Day1_TicketDetective.Common;
using Day1_TicketDetective.Data;
using Day1_TicketDetective.Models;

namespace Day1_TicketDetective.Solutions
{
    // NOTE FOR THE TRAINER: this is the "after" / corrected version for Ticket #4.
    // Compare it against Services/PatientService.cs.
    public class PatientServiceFixed : Services.IPatientService
    {
        private readonly ClinicDatabase _db;

        public PatientServiceFixed(ClinicDatabase db)
        {
            _db = db;
        }

        // Ticket #4 - fixed: rejects registering a patient whose National ID
        // already exists, instead of silently creating a duplicate record.
        public Result<Patient> RegisterPatient(string fullName, string phone, string nationalId)
        {
            var existing = _db.Patients.FirstOrDefault(p => p.NationalId == nationalId);
            if (existing != null)
                return Result<Patient>.Failure(
                    $"A patient with National ID {nationalId} is already registered (Patient #{existing.Id}: {existing.FullName}).");

            var patient = new Patient
            {
                Id = _db.NextPatientId(),
                FullName = fullName,
                Phone = phone,
                NationalId = nationalId
            };

            _db.Patients.Add(patient);

            return Result<Patient>.Success(patient, "Patient registered successfully.");
        }
    }
}
