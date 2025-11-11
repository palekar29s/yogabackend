namespace yogabackend.Model
{
    public class Wanderimage
    {
        public int WanderingId { get; set; }   // matches wandering_id
        public int StudentId { get; set; }     // matches student_id
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte[] LostThingImage { get; set; }
    }
}
