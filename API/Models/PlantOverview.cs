using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class PlantOverview :Common
    {
        public int PlantNameId { get; set; }
        public int MoistureLevel { get; set; }

		public int sensorId { get; set; }
        public int ArduinoId { get; set; }

	}
}
