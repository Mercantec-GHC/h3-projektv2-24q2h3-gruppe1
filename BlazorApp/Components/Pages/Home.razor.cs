using API.Models;
using BlazorApp.Containers;
using BlazorBootstrap;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using System.Text;

namespace BlazorApp.Components.Pages
{
    public partial class Home
    {
        #region Top Level Variables
        string message = "";
        string errorMessage = "";

        // Modal User Input Variables
        string email = "";
        string username = "";
        string password = "";

        string newEmail = "";
        string newUsername = "";
        string newPassword = "";

        bool usernameCheck = false;
        bool passwordCheck = false;

        // Pie Variables
        public string[]? backgroundColors;
        public PieChart pieChart = default!;
        public PieChartOptions pieChartOptions = default!;
        public BlazorBootstrap.ChartData chartData = default!;

        public int datasetsCount = 0;
        public int dataLabelsCount = 0;

        // Random Variable
        public Random random = new();

        // Plants Variables
        public List<Plant>? plants;
        public List<Setting>? settingList;

        public UserLoginRequest userLogin = new UserLoginRequest();
        public UserSignUpRequest userSignup = new UserSignUpRequest();
        public User userProfile = new User();

        public Plant plantProfile = new Plant();
        
        public bool IsAutoChecked = true;
        public bool IsManualChecked = false;

     
        
        // Connection
        private HttpClient client = new HttpClient() { BaseAddress = new Uri("https://h3-projektv2-24q2h3-gruppe1-rolc.onrender.com") };
        private int numberOfDatasets;
        #endregion

        // -------------------------- Plants ---------------------------- //

        #region Plants Create, Edit, Get Plants & Settings
        // Create plant to database
        //need to get user id
        public async Task HandleCreatePlant()
        {
    
            if (string.IsNullOrWhiteSpace(plantProfile.PlantName))
            {
                errorMessage = "invalid plant name";
            }

            if (!plantProfile.PlantName.All(char.IsLetterOrDigit))
            {
                errorMessage = "invalid input cant contain speical characters try again";
            }

            if (plantProfile.MinWaterLevel < 0 || plantProfile.MinWaterLevel > 100)
            {
                errorMessage = "invalid minWaterLevel input try again over or under limit";
            }

            if (plantProfile.MaxWaterLevel < 0 || plantProfile.MaxWaterLevel > 100)
            {
                errorMessage = "invalid MaxWaterLevel input try again over or under limit";
            }

            else
            {
                plantProfile.UserId = AccountSession.UserSession.Id;

                //make post request
                string json = System.Text.Json.JsonSerializer.Serialize(plantProfile);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Plants", content);


                if (response.IsSuccessStatusCode)
                {
                    message = "plant succesfully created";
                    NavigationManager.NavigateTo("/");
                }
            }
        }

        // Edit plant info for the database
        public async Task HandleEditPlant()
        {
          
            //if (string.IsNullOrWhiteSpace(plantProfile.PlantName))
            //{
            //    errorMessage = "invalid plant name";
            //}

            //if (!plantProfile.PlantName.All(char.IsLetterOrDigit))
            //{
            //    errorMessage = "invalid input cant contain speical characters try again";
            //}

            if (plantProfile.MinWaterLevel < 0 || plantProfile.MinWaterLevel > 100)
            {
                errorMessage = "invalid minWaterLevel input try again over or under limit";
            }

            if (plantProfile.MaxWaterLevel < 0 || plantProfile.MaxWaterLevel > 100)
            {
                errorMessage = "invalid MaxWaterLevel input try again over or under limit";
            }

            else
            {
                plantProfile.Id = selectedEditPlantId;
                plantProfile.PlantName = selectedEditPlant;
                //make put request
                string json = System.Text.Json.JsonSerializer.Serialize(plantProfile);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/Plants/{plantProfile.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    message = "plant succesfully created";
                    NavigationManager.NavigateTo("/");
                }
            }
        }
        //make put request in settings then we change plant name in setup sensor
        // ------------- Get -------------- //

        // Get the list of plants for drop down menu
        public async Task GetListOfPlants()
        {
            // It takes the list of plants we have in our API via the endpoint "api/Plants"
            try
            {
                if (AccountSession.UserSession != null)
                {
                    var allPlants = await client.GetFromJsonAsync<List<Plant>>("api/Plants");

                    // Filter plants where UserId is 0 and AccountSessionId matches
                    var filteredPlants = allPlants.Where(plant => plant.UserId == 0 || plant.UserId == AccountSession.UserSession.Id).ToList();

                    // Assign filtered plants to the plants list
                    plants = filteredPlants;
                }
                else
                {
                    var allPlants = await client.GetFromJsonAsync<List<Plant>>("api/Plants");

                    // Filter plants where UserId is 0 and AccountSessionId matches
                    var filteredPlants = allPlants.Where(plant => plant.UserId == 0).ToList();

                    // Assign filtered plants to the plants list
                    plants = filteredPlants;
                }

            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error fetching plants: {ex.Message}");
            }
        }

        // Get the settings the user has set
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
        #endregion

        // --------------------------- Sensor --------------------------- //

        #region Sensor
        //
        public async Task SetupSensor()
        {

        }
        #endregion

        // ----------------------------- Pie ---------------------------- //

        #region Pie Chart
        //
        public async Task AddDatasetAsync()
        {
            if (chartData is null || chartData.Datasets is null) return;

            var chartDataset = GetRandomPieChartDataset();
            chartData = await pieChart.AddDatasetAsync(chartData, chartDataset, pieChartOptions);
        }
        
        //
        public async Task AddDataAsync()
        {
            if (dataLabelsCount >= 12)
                return;

            if (chartData is null || chartData.Datasets is null)
                return;

            var data = new List<IChartDatasetData>();
            foreach (var dataset in chartData.Datasets)
            {
                if (dataset is PieChartDataset pieChartDataset)
                    data.Add(new PieChartDatasetData(pieChartDataset.Label, random.Next(0, 100), backgroundColors![dataLabelsCount]));
            }

            chartData = await pieChart.AddDataAsync(chartData, GetNextDataLabel(), data);

            dataLabelsCount += 1;
        }

        // The auto or manual mode toggle for the Arduino 
        //make a put request then auto changes a put request
        public void Toggle(char switchName)
        {
            var datasets = new List<IChartDataset>();

            for (var index = 0; index < numberOfDatasets; index++)
            {
                datasets.Add(GetRandomPieChartDataset());
            }

           // return datasets;
        }

        //
        public PieChartDataset GetRandomPieChartDataset()
        {
            datasetsCount += 1;
            return new() { Label = $"Team {datasetsCount}", Data = GetRandomData(), BackgroundColor = GetRandomBackgroundColors() };
        }

        //
        public List<double> GetRandomData()
        {
            var data = new List<double>();
            for (var index = 0; index < dataLabelsCount; index++)
            {
                data.Add(random.Next(0, 100));
            }

            return data;
        }

        //
        public List<string> GetRandomBackgroundColors()
        {
            var colors = new List<string>();
            for (var index = 0; index < dataLabelsCount; index++)
            {
                colors.Add(backgroundColors![index]);
            }

            return colors;
        }

        //
        public List<string> GetDefaultDataLabels(int numberOfLabels)
        {
            var labels = new List<string>();
            for (var index = 0; index < numberOfLabels; index++)
            {
                labels.Add(GetNextDataLabel());
                dataLabelsCount += 1;
            }

            return labels;
        }

        //
        public string GetNextDataLabel() => $"Product {dataLabelsCount + 1}";

        //
        public string GetNextDataBackgrounfColor() => backgroundColors![dataLabelsCount];
        #endregion
    }
}
