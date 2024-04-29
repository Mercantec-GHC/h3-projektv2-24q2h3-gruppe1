using API.Models;
using Newtonsoft.Json;

namespace BlazorApp.Components.Pages
{
    public partial class Home
    {
        string UserApi = "https://h3-projektv2-24q2h3-gruppe1-sqve.onrender.com/api/Users";

        public async Task<User> GetSensorMoistureAsync(int moisture)
        {

            UserApi = $"https://h3-projektv2-24q2h3-gruppe1-sqve.onrender.com/api/Users/{moisture}";

            HttpResponseMessage response = new HttpResponseMessage();

            User User = new User();

            string jsonData = "";

            try
            {
                response = await UserClient.GetAsync(UserApi);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                jsonData = await response.Content.ReadAsStringAsync();
                User = JsonConvert.DeserializeObject<User>(jsonData);
                if (User != null)
                {
                    return User;
                }
            }

            catch (Exception)
            {
                return null;
            }

            return null;
        }
        // Define sample data
        public List<DataPoint> ChartData { get; set; }

        // Initialize sample data in the OnInitialized method
        protected override void OnInitialized()
        {
            // Sample data initialization
            ChartData = new List<DataPoint>
        {
            new DataPoint { XValue = "Jan", YValue = 30 },
            new DataPoint { XValue = "Feb", YValue = 40 },
            new DataPoint { XValue = "Mar", YValue = 50 },
            new DataPoint { XValue = "Apr", YValue = 60 },
            new DataPoint { XValue = "May", YValue = 70 }
        };
        }

        // Define a class for data points
        public class DataPoint
        {
            public string XValue { get; set; }
            public double YValue { get; set; }
        }
    }
}
