namespace yogabackend.Model
{
    public class Login
    {
        public int LoginId { get; set; }
        public required string Username { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }


    }
}
