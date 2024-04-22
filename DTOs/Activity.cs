namespace GoalQuest.DTOs
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public bool Important { get; set; }
    }
}
