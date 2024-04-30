using static System.Net.WebRequestMethods;
using API.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Syncfusion.Blazor.Data;


namespace BlazorApp.Components.Pages
{
    public partial class Sensor
    {
        private HttpClient client = new HttpClient() { BaseAddress = new Uri("https://h3-projektv2-24q2h3-gruppe1-sqve.onrender.com/") };

        public List<Plant>? plants;
        public async Task GetListOfPlants()
        {
            // Use the 'client' HttpClient instance declared above to make the request
            plants = await client.GetFromJsonAsync<List<Plant>>("api/Plants");
        }
        string abe = "a0";
    }
}
