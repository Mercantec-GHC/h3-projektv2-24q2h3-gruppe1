
namespace watervand.Models
{
    public class plantsensor 
    {
        public int Id { get; set; }
        sensor sensor_id { get; set; }
        plant plant_id { get; set; }
        User user_id { get; set; }
        
    }
}
