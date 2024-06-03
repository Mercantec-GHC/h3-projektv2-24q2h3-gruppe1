namespace API.Models
{
	public class Arduino : Common
	{
		public string Sensor1Name { get; set; } = "sensor1";
		public string Sensor2Name { get; set; } = "sensor2";
		public int SensorId1 { get; set; } = 1;
		public int SensorId2 { get; set; } = 2;
		public int UserId { get; set; }
    }

	public class PutSensorName
	{
		public string Sensor1Name { get; set; }
		public string Sensor2Name { get; set; }
	}
	public class CreateArduino
	{
		public int SensorId1 { get; set; }
		public int SensorId2 { get; set; }
		public string Sensor1Name { get; set; }
		public string Sensor2Name { get; set; }
	}
	public class AddUserToArduino
	{
		public int UserId { get; set; }

	}
}
