namespace Movie_App_MinimalApi.DTOs
{
    public class ActorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string? ActorPic { get; set; }//HERE  URL TO THE ACTOR PICTURE WILL BE SENT TO THE CLIENT
    }
}
