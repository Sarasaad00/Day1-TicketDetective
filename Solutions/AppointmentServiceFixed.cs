using Day1_TicketDetective.Common;
using Day1_TicketDetective.Data;
using Day1_TicketDetective.Models;

namespace Day1_TicketDetective.Solutions
{
    // NOTE FOR THE TRAINER: this is the "after" / corrected version, once the
    // group has identified what each ticket left unwritten. Compare it method
    // by method against Services/AppointmentService.cs.
    public class AppointmentServiceFixed : Services.IAppointmentService
    {
        private readonly ClinicDatabase _db;

        public AppointmentServiceFixed(ClinicDatabase db)
        {
            _db = db;
        }

        // Ticket #1 - fixed: checks real time-range overlap (not just exact
        // equality), checks the doctor's working hours, and rejects past dates.
        public Result<Appointment> BookAppointment(int patientId, int doctorId, DateTime startTime)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.Id == patientId);
            if (patient == null)
                return Result<Appointment>.Failure("Patient not found.");

            var doctor = _db.Doctors.FirstOrDefault(d => d.Id == doctorId);
            if (doctor == null)
                return Result<Appointment>.Failure("Doctor not found.");

            var duration = TimeSpan.FromMinutes(30);

            if (startTime < DateTime.Now)
                return Result<Appointment>.Failure("Cannot book an appointment in the past.");

            var workStart = startTime.Date.AddHours(doctor.WorkStartHour);
            var workEnd = startTime.Date.AddHours(doctor.WorkEndHour);
            if (startTime < workStart || startTime.Add(duration) > workEnd)
                return Result<Appointment>.Failure(
                    $"Requested time is outside Dr. {doctor.FullName}'s working hours " +
                    $"({doctor.WorkStartHour}:00-{doctor.WorkEndHour}:00).");

            var newEnd = startTime.Add(duration);
            bool hasOverlap = _db.Appointments.Any(a =>
                a.DoctorId == doctorId &&
                a.Status == AppointmentStatus.Booked &&
                startTime < a.StartTime.Add(a.Duration) &&
                a.StartTime < newEnd);

            if (hasOverlap)
                return Result<Appointment>.Failure("This doctor already has an overlapping appointment at this time.");

            var appointment = new Appointment
            {
                Id = _db.NextAppointmentId(),
                PatientId = patientId,
                DoctorId = doctorId,
                StartTime = startTime,
                Duration = duration,
                Status = AppointmentStatus.Booked
            };

            _db.Appointments.Add(appointment);

            return Result<Appointment>.Success(appointment, "Appointment booked successfully.");
        }

        // Ticket #2 - fixed: rejects cancelling an already-cancelled appointment,
        // and rejects cancelling an appointment whose time has already passed.
        public Result CancelAppointment(int appointmentId)
        {
            var appointment = _db.Appointments.FirstOrDefault(a => a.Id == appointmentId);
            if (appointment == null)
                return Result.Failure("Appointment not found.");

            if (appointment.Status == AppointmentStatus.Cancelled)
                return Result.Failure("This appointment is already cancelled.");

            if (appointment.StartTime < DateTime.Now)
                return Result.Failure("Cannot cancel an appointment that has already happened.");

            appointment.Status = AppointmentStatus.Cancelled;

            return Result.Success("Appointment cancelled successfully.");
        }

        // Ticket #3 - fixed: excludes any slot that has already passed when the
        // requested date is today, and excludes slots that overlap a booked appointment.
        public Result<List<DateTime>> GetAvailableSlots(int doctorId, DateTime date)
        {
            var doctor = _db.Doctors.FirstOrDefault(d => d.Id == doctorId);
            if (doctor == null)
                return Result<List<DateTime>>.Failure("Doctor not found.");

            var duration = TimeSpan.FromMinutes(30);
            var allSlots = new List<DateTime>();
            for (int hour = doctor.WorkStartHour; hour < doctor.WorkEndHour; hour++)
            {
                allSlots.Add(date.Date.AddHours(hour));
            }

            var bookedAppointments = _db.Appointments
                .Where(a => a.DoctorId == doctorId && a.Status == AppointmentStatus.Booked)
                .ToList();

            var availableSlots = allSlots
                .Where(slot => slot >= DateTime.Now) // exclude slots already in the past
                .Where(slot => !bookedAppointments.Any(a =>
                    slot < a.StartTime.Add(a.Duration) && a.StartTime < slot.Add(duration)))
                .ToList();

            return Result<List<DateTime>>.Success(availableSlots);
        }

        // Ticket #5 - fixed: excludes cancelled appointments and sorts by time.
        public Result<List<Appointment>> GetDoctorScheduleForDay(int doctorId, DateTime date)
        {
            var schedule = _db.Appointments
                .Where(a => a.DoctorId == doctorId
                            && a.StartTime.Date == date.Date
                            && a.Status == AppointmentStatus.Booked)
                .OrderBy(a => a.StartTime)
                .ToList();

            return Result<List<Appointment>>.Success(schedule);
        }
    }
}
