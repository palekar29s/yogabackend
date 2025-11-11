namespace yogabackend.Model
{
    public class Student
    {

        public int StudentId { get; set; }   // Matches student_id
        public string Name { get; set; }
        public string CurrentAcademicYear { get; set; }
        public string RollNumber { get; set; }
        public DateTime? Dob { get; set; }   // Nullable date
        public string Barcode { get; set; }
        public byte[] IdImage1 { get; set; }
        public byte[] IdImage2 { get; set; }
        public int? LoginId { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
