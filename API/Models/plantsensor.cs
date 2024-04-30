namespace API.Models
{
    public class PlantSensor : Common
    {
        public User  User_id { get; set; }
        public Sensor Sensor_id { get; set; }
        public Plant Plant_id { get; set; }
    }
}
