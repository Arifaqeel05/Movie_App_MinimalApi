namespace Movie_App_MinimalApi.Entity
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; } = null;
        public DateTime DateOfBirth { get; set; }
        public string ActorPic { get; set; }
    }
}
