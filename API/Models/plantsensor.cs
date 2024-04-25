namespace API.Models
{
    public class PlantSensor : Common
    {
        public User  User_id { get; set; }
        public Sensor Sensor_id { get; set; }

        public PlantOverview PlantOverview_id { get; set; }
    }
}
