namespace API.Models
{
    public class Setting :Common
    {
       public int UserId {  get; set; }
       public bool AutoMode { get; set; } = true;
        public string Sensor1Name { get; set; } = "sensor1";
       public string Sensor2Name { get; set; } = "sensor2";

    }
}
