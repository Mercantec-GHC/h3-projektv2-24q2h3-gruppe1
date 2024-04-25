namespace API.Models
{
    public class Plants : Common
    {
        public string PlantName { get; set; }
        public int MinWaterLevel { get; set; }
        public int MaxWaterLevel { get; set; }
    }
}
