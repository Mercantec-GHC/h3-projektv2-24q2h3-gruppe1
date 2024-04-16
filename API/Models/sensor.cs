using System.ComponentModel.DataAnnotations;

namespace watervand.Models
{
    public class sensor
    {
        [Key]
        public int sensor_id {  get; set; }
        public string name { get; set;}
    }
}
