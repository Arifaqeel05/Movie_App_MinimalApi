namespace Movie_App_MinimalApi.DTOs
{
    public class CreateUpdateActorDTO
    {
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public  IFormFile? ActorPic { get; set; } //HERE WE WILL RECEIVE THE ACTOR PICTURE FROM THE CLIENT
    }
}
