using Day1_TicketDetective.Models;

namespace Day1_TicketDetective.Data
{
    // Acts as a tiny normalized "database": each table is a list of POCOs,
    // related to each other by Id only - just like real database tables would be.
    public class ClinicDatabase
    {
        public List<Patient> Patients { get; } = new();
        public List<Doctor> Doctors { get; } = new();
        public List<Appointment> Appointments { get; } = new();

        private int _nextPatientId = 1;
        private int _nextDoctorId = 1;
        private int _nextAppointmentId = 1;

        public int NextPatientId() => _nextPatientId++;
        public int NextDoctorId() => _nextDoctorId++;
        public int NextAppointmentId() => _nextAppointmentId++;

        public static ClinicDatabase SeedSampleData()
        {
            var db = new ClinicDatabase();

            var doctor1 = new Doctor { Id = db.NextDoctorId(), FullName = "Dr. Samir Fathy", Specialty = "General Medicine", WorkStartHour = 9, WorkEndHour = 17 };
            var doctor2 = new Doctor { Id = db.NextDoctorId(), FullName = "Dr. Laila Hassan", Specialty = "Pediatrics", WorkStartHour = 10, WorkEndHour = 18 };
            db.Doctors.Add(doctor1);
            db.Doctors.Add(doctor2);

            var patient1 = new Patient { Id = db.NextPatientId(), FullName = "Mona Ali", Phone = "01001234567", NationalId = "29001010112233" };
            var patient2 = new Patient { Id = db.NextPatientId(), FullName = "Ahmed Kader", Phone = "01109876543", NationalId = "29102020223344" };
            db.Patients.Add(patient1);
            db.Patients.Add(patient2);

            var tomorrow = DateTime.Today.AddDays(1);

            // Existing appointment tomorrow at 10:00 - used to test booking conflicts.
            db.Appointments.Add(new Appointment
            {
                Id = db.NextAppointmentId(),
                PatientId = patient1.Id,
                DoctorId = doctor1.Id,
                StartTime = tomorrow.AddHours(10),
                Duration = TimeSpan.FromMinutes(30),
                Status = AppointmentStatus.Booked
            });

            // Added AFTER the 10:00 one, but scheduled EARLIER in the day (09:00) -
            // on purpose, so an unsorted schedule becomes visibly obvious.
            db.Appointments.Add(new Appointment
            {
                Id = db.NextAppointmentId(),
                PatientId = patient2.Id,
                DoctorId = doctor1.Id,
                StartTime = tomorrow.AddHours(9),
                Duration = TimeSpan.FromMinutes(30),
                Status = AppointmentStatus.Booked
            });

            // A cancelled appointment tomorrow - used to test whether cancelled
            // appointments are correctly excluded from the doctor's daily schedule.
            db.Appointments.Add(new Appointment
            {
                Id = db.NextAppointmentId(),
                PatientId = patient1.Id,
                DoctorId = doctor1.Id,
                StartTime = tomorrow.AddHours(14),
                Duration = TimeSpan.FromMinutes(30),
                Status = AppointmentStatus.Cancelled
            });

            // An appointment already in the past (2 hours ago) - used to test
            // cancelling an appointment that has already happened.
            db.Appointments.Add(new Appointment
            {
                Id = db.NextAppointmentId(),
                PatientId = patient2.Id,
                DoctorId = doctor2.Id,
                StartTime = DateTime.Now.AddHours(-2),
                Duration = TimeSpan.FromMinutes(30),
                Status = AppointmentStatus.Booked
            });

            return db;
        }
    }
}
