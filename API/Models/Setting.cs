namespace API.Models
{
    public class Setting :Common
    {
       public int UserId {  get; set; }
       public bool AutoMode { get; set; } = true;
       public string Sensor1Name { get; set; } = "sensor1";
       public string Sensor2Name { get; set; } = "sensor2";
       public string? SelectedPlant1 { get; set; }
       public string? SelectedPlant2 { get; set; }
        public int SensorID1 { get; set; }
        public int SensorID2 { get; set;}

    }
    public class PutSettings
    {
        public string Sensor1Name { get; set; }
        public string Sensor2Name { get; set; }
    }

    public class PutMode
    {
        public bool AutoMode { get; set; }
    }

    public class PutSeletedPlants
    {
        public string? SelectedPlant1 { get; set; }
        public string? SelectedPlant2 { get; set; }
    }

    public class PutSensorID
    {
        public int SensorID1 { get; set; }
        public int SensorID2 { get; set; }
    }
}
