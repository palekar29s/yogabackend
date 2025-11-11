namespace yogabackend.Model
{
    public class Contactus
    {
        public int CommentId { get; set; }
        public int WanderingId { get; set; }
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
