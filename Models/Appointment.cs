namespace Day1_TicketDetective.Models
{
    public enum AppointmentStatus
    {
        Booked,
        Cancelled
    }

    public class Appointment
    {
        public int Id { get; set; }

        // Normalized: we store the foreign keys, not full copies of Patient/Doctor.
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(30);
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;
    }
}
