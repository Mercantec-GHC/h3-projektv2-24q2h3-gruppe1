
using System.ComponentModel.DataAnnotations;

namespace watervand.Models
{
    public class plant
    {
        [Key]
        public int Id { get; set; }
        public string plantName { get; set; }
        public int moisture { get; set; }
    }
}
