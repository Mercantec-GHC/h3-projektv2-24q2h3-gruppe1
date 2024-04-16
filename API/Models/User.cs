using System.ComponentModel.DataAnnotations;

namespace watervand.Models
{
    public class User
    {
        [Key]
        public int user_id {  get; set; }
       public string username { get; set; }
        public string hashed_password { get; set; }
        public string email { get; set; }
    }
}
