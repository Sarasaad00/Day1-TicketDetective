using Day1_TicketDetective.Common;
using Day1_TicketDetective.Models;

namespace Day1_TicketDetective.Services
{
    public interface IAppointmentService
    {
        Result<Appointment> BookAppointment(int patientId, int doctorId, DateTime startTime);
        Result CancelAppointment(int appointmentId);
        Result<List<DateTime>> GetAvailableSlots(int doctorId, DateTime date);
        Result<List<Appointment>> GetDoctorScheduleForDay(int doctorId, DateTime date);
    }
}
