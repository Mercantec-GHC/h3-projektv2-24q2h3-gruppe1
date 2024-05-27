using API.Models;
using Syncfusion.Blazor.Charts.Chart.Internal;
using System.Numerics;

namespace BlazorApp.Components.Pages
{
    public partial class Settings
    {
        private HttpClient client = new HttpClient() { BaseAddress = new Uri("https://h3-projektv2-24q2h3-gruppe1-sqve.onrender.com/") };

        private bool IsAutoChecked = true;
        private bool IsManualChecked = false;
        public List<Setting>? settingList;
   
        //make get request
        public async Task GetListOfSettings()
        {
            try
            {
                settingList = await client.GetFromJsonAsync<List<Setting>>("api/Settings");
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error fetching Settings: {ex.Message}");
            }
        }
        private void Toggle(char switchName)
        {
            if (!IsManualChecked && !IsAutoChecked)
            {
                IsAutoChecked = true;
            }
            else
            {
                IsAutoChecked = !IsAutoChecked;
                IsManualChecked = !IsManualChecked;
            }
        }
    }
}
