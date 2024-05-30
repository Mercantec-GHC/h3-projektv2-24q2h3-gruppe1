namespace API.Models
{
    public class Setting :Common
    {
       public int UserId {  get; set; }
       public bool AutoMode { get; set; } = true;

       public string? SelectedPlant1 { get; set; }
       public string? SelectedPlant2 { get; set; }
   

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


}
