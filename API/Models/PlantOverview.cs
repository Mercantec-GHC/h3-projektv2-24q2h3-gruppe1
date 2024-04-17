using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class PlantOverview : Common2
    {
        public string PlantName { get; set; }
        public int MoistureLevel { get; set; }
        public int MinWaterLevel { get; set; }
        public int MaxWaterLevel { get; set; }
    }
}
