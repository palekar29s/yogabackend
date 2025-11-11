namespace yogabackend.Model
{
    public class Contact
    {
        public int ContactId { get; set; }   // Matches contact_id
        public string Name { get; set; }
        public string Email { get; set; }
       
        public string Message { get; set; }
        public DateTime? CreatedAt { get; set; }   // Nullable date
    }
}
