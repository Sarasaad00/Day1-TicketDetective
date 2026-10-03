using Day1_TicketDetective.Common;
using Day1_TicketDetective.Models;

namespace Day1_TicketDetective.Services
{
    public interface IPatientService
    {
        Result<Patient> RegisterPatient(string fullName, string phone, string nationalId);
    }
}
