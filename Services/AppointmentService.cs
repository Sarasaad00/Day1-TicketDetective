using Day1_TicketDetective.Common;
using Day1_TicketDetective.Data;
using Day1_TicketDetective.Models;

namespace Day1_TicketDetective.Services
{
    // NOTE FOR THE TRAINER: this is the "before" / buggy version, intentionally
    // matching only what each ticket says literally. See Solutions/AppointmentServiceFixed.cs
    // for the corrected version once the group has discussed each ticket.
    public class AppointmentService : IAppointmentService
    {
        private readonly ClinicDatabase _db;

        public AppointmentService(ClinicDatabase db)
        {
            _db = db;
        }

        // Ticket #1: Book Appointment
        public Result<Appointment> BookAppointment(int patientId, int doctorId, DateTime startTime)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.Id == patientId);
            if (patient == null)
                return Result<Appointment>.Failure("Patient not found.");

            var doctor = _db.Doctors.FirstOrDefault(d => d.Id == doctorId);
            if (doctor == null)
                return Result<Appointment>.Failure("Doctor not found.");

            bool conflict = _db.Appointments.Any(a =>
                a.DoctorId == doctorId &&
                a.Status == AppointmentStatus.Booked &&
                a.StartTime == startTime);

            if (conflict)
                return Result<Appointment>.Failure("This doctor already has an appointment at this exact time.");

            var appointment = new Appointment
            {
                Id = _db.NextAppointmentId(),
                PatientId = patientId,
                DoctorId = doctorId,
                StartTime = startTime,
                Status = AppointmentStatus.Booked
            };

            _db.Appointments.Add(appointment);

            return Result<Appointment>.Success(appointment, "Appointment booked successfully.");
        }

        // Ticket #2: Cancel Appointment
        public Result CancelAppointment(int appointmentId)
        {
            var appointment = _db.Appointments.FirstOrDefault(a => a.Id == appointmentId);
            if (appointment == null)
                return Result.Failure("Appointment not found.");

            appointment.Status = AppointmentStatus.Cancelled;

            return Result.Success("Appointment cancelled successfully.");
        }

        // Ticket #3: Get Available Slots
        public Result<List<DateTime>> GetAvailableSlots(int doctorId, DateTime date)
        {
            var doctor = _db.Doctors.FirstOrDefault(d => d.Id == doctorId);
            if (doctor == null)
                return Result<List<DateTime>>.Failure("Doctor not found.");

            var allSlots = new List<DateTime>();
            for (int hour = doctor.WorkStartHour; hour < doctor.WorkEndHour; hour++)
            {
                allSlots.Add(date.Date.AddHours(hour));
            }

            var bookedTimes = _db.Appointments
                .Where(a => a.DoctorId == doctorId && a.Status == AppointmentStatus.Booked)
                .Select(a => a.StartTime)
                .ToHashSet();

            var availableSlots = allSlots.Where(slot => !bookedTimes.Contains(slot)).ToList();

            return Result<List<DateTime>>.Success(availableSlots);
        }

        // Ticket #5: Doctor's Daily Schedule
        public Result<List<Appointment>> GetDoctorScheduleForDay(int doctorId, DateTime date)
        {
            var schedule = _db.Appointments
                .Where(a => a.DoctorId == doctorId && a.StartTime.Date == date.Date)
                .ToList();

            return Result<List<Appointment>>.Success(schedule);
        }
    }
}
