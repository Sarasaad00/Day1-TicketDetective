namespace Day1_TicketDetective.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;

        // Working hours, e.g. 9 -> 17 means the doctor works from 9 AM to 5 PM.
        public int WorkStartHour { get; set; }
        public int WorkEndHour { get; set; }
    }
}
