using Day1_TicketDetective.Common;
using Day1_TicketDetective.Data;
using Day1_TicketDetective.Models;

namespace Day1_TicketDetective.Services
{
    // NOTE FOR THE TRAINER: this is the "before" / buggy version - see
    // Solutions/PatientServiceFixed.cs for the corrected version.
    public class PatientService : IPatientService
    {
        private readonly ClinicDatabase _db;

        public PatientService(ClinicDatabase db)
        {
            _db = db;
        }

        // Ticket #4: Register New Patient
        public Result<Patient> RegisterPatient(string fullName, string phone, string nationalId)
        {
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
