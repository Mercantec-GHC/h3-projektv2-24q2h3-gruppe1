using API.Models;
using BlazorApp.Containers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol.Plugins;
using System.Text;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Components.Pages
{
    public partial class Home
    {
        #region Top Level Variables
        string message = "";
        string errorMessageCreatePlant = "";
        string errorMessageEditPlant = "";
        string errorMessageDeletePlant = "";
        string errorMessageSignup = "";
        string errorMessage = "";

        bool usernameCheck = false;
        bool passwordCheck = false;
     
        public List<Plant>? plants;
        public List<Plant>? Useronlyplants;

        public List<Setting>? settingList;
        public List<Arduino>? arduinoList;

        public UserSignUpRequest userSignup = new UserSignUpRequest();
        public User userProfile = new User();
        public AddUserToArduino addUserArduino = new AddUserToArduino();

        public Plant plantProfile = new Plant();
        public Plant createPlantProfile = new Plant();
        public PutSensorName settings = new PutSensorName();
        public PutMode mode = new PutMode();
        public PutSeletedPlants selectedPlants = new PutSeletedPlants();
 

        public bool IsAutoChecked = true;
        public bool IsManualChecked = false;

        private HttpClient client = new HttpClient() { BaseAddress = new Uri("https://h3-projektv2-24q2h3-gruppe1-pjo3.onrender.com") };
        #endregion

        // -------------------------- Plants ---------------------------- //

        // Create plant to database
        //need to get user id
        public async Task HandleCreatePlant()
        {

            if (string.IsNullOrWhiteSpace(createPlantProfile.PlantName))
            {
                errorMessageCreatePlant = "invalid plant name";
            }

            if (!createPlantProfile.PlantName.All(char.IsLetterOrDigit))
            {
                errorMessageCreatePlant = "invalid input cant contain speical characters try again";
            }

            if (createPlantProfile.MinWaterLevel < 0 || createPlantProfile.MinWaterLevel > 100)
            {
                errorMessageCreatePlant = "invalid minWaterLevel input try again over or under limit";
            }

            if (createPlantProfile.MaxWaterLevel < 0 || createPlantProfile.MaxWaterLevel > 100)
            {
                errorMessageCreatePlant = "invalid MaxWaterLevel input try again over or under limit";
            }

            else
            {
                createPlantProfile.UserId = AccountSession.UserSession.Id;

                //make post request
                string json = System.Text.Json.JsonSerializer.Serialize(createPlantProfile);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Plants", content);


                if (response.IsSuccessStatusCode)
                {
                    message = "plant succesfully created";
                    await JS.InvokeVoidAsync("closeModal", "myModalCreatePlant");
                    //Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
                    NavigationManager.NavigateTo("/login");
                    NavigationManager.NavigateTo("/"); NavigationManager.NavigateTo("/login");
                    NavigationManager.NavigateTo("/");
                }
            }
        }

        // Edit plant info for the database
        public async Task HandleEditPlant()
        {
            if (string.IsNullOrWhiteSpace(plantProfile.PlantName))
            {
                errorMessageEditPlant = "invalid plant name";
            }

            if (!plantProfile.PlantName.All(char.IsLetterOrDigit))
            {
                errorMessageEditPlant = "invalid input cant contain speical characters try again";
            }

            if (plantProfile.MinWaterLevel < 0 || plantProfile.MinWaterLevel > 100)
            {
                errorMessageEditPlant = "invalid minWaterLevel input try again over or under limit";
            }

            if (plantProfile.MaxWaterLevel < 0 || plantProfile.MaxWaterLevel > 100)
            {
                errorMessageEditPlant = "invalid MaxWaterLevel input try again over or under limit";
            }

            else
            {
                plantProfile.Id = selectedEditPlantId;
                //make put request
                string json = System.Text.Json.JsonSerializer.Serialize(plantProfile);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/Plants/{plantProfile.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    message = "plant succesfully created";
                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    errorMessageEditPlant = "select a plant ";
                }
            }
        }

        // Edit plant info for the database
        public async Task HandlePlantDelete()
        {
            plantProfile.Id = selectedEditPlantId;
            //make put request
            string json = System.Text.Json.JsonSerializer.Serialize(plantProfile);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.DeleteAsync($"api/Plants/{plantProfile.Id}");

            if (response.IsSuccessStatusCode)
            {
                message = "plant succesfully created";

                await JS.InvokeVoidAsync("closeModal", "myModalEditPlant");

                NavigationManager.NavigateTo("/login");
                NavigationManager.NavigateTo("/"); NavigationManager.NavigateTo("/login");
                NavigationManager.NavigateTo("/");
            }
            else
            {
                errorMessageDeletePlant = "Select a valid plant";
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
                    var allUserPlants = await client.GetFromJsonAsync<List<Plant>>("api/Plants");

                    // Filter plants where UserId is 0 and AccountSessionId matches
                    var filteredPlants = allPlants.Where(plant => plant.UserId == 0 || plant.UserId == AccountSession.UserSession.Id).ToList();
                    var filtereduseronlyPlants = allUserPlants.Where(Useronlyplants => Useronlyplants.UserId == AccountSession.UserSession.Id).ToList();

                    // Assign filtered plants to the plants list
                    plants = filteredPlants;
                    Useronlyplants = filtereduseronlyPlants;
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
                if (AccountSession.UserSession != null)
                {
                    settingList = await client.GetFromJsonAsync<List<Setting>>("api/Settings");
                    arduinoList = await client.GetFromJsonAsync<List<Arduino>>("api/arduino");
                    var filteredUserOnlySettings = settingList.Where(userOnlySettings => userOnlySettings.UserId == AccountSession.UserSession.Id).ToList();
                    var filteredUserOnlyarduino = arduinoList.Where(userOnlySettings => userOnlySettings.UserId == AccountSession.UserSession.Id).ToList();

                    if (filteredUserOnlySettings.Any())
                    {
                        var userSettings = filteredUserOnlySettings[0];
                        settingsId = userSettings.Id; // Save the settings ID
                        sensorName1 = filteredUserOnlyarduino[0].Sensor1Name;
                        sensorName2 = filteredUserOnlyarduino[0].Sensor2Name;
                        mode.AutoMode = filteredUserOnlySettings[0].AutoMode;
                        current1plant = filteredUserOnlySettings[0].SelectedPlant1;
                        current2plant = filteredUserOnlySettings[0].SelectedPlant2;
                        originalSensorName1 = filteredUserOnlyarduino[0].Sensor1Name;
                        originalSensorName2 = filteredUserOnlyarduino[0].Sensor2Name;
                        arduinoID = filteredUserOnlyarduino[0].Id;

                    }

                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error fetching Settings: {ex.Message}");
            }
        }

        public async Task PutSensorNames()
        {
            settings.Sensor2Name = sensorName2;
            settings.Sensor1Name = sensorName1;
            //make put request
            string json = System.Text.Json.JsonSerializer.Serialize(settings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/arduino/sensorname/{arduinoID}", content);

            if (response.IsSuccessStatusCode)
            {
              await GetListOfSettings();
                message = "Updated name";
                isEditingSensorName2 = false;
                isEditingSensorName1 = false;
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent);
                errorMessage = errorResponse?.Detail ?? "An error occurred while saving settings.";
            }
        }

        public async Task PutModeState()
        {
            string json = System.Text.Json.JsonSerializer.Serialize(mode);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/settings/mode/{settingsId}", content);

            if (response.IsSuccessStatusCode)
            {
                message = "Updated name";
                await JS.InvokeVoidAsync("closeModal", "myModalSettings");
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent);
                errorMessage = errorResponse?.Detail ?? "An error occurred while saving mode.";
            }
        }

		public async Task PutAddUser()
		{
            addUserArduino.UserId = AccountSession.UserSession.Id;

            string json = System.Text.Json.JsonSerializer.Serialize(addUserArduino);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await client.PutAsync($"api/arduino/adduserarduino/{arduinoID}", content);

			if (response.IsSuccessStatusCode)
			{
				message = "Updated name";
				await JS.InvokeVoidAsync("closeModal", "myModalSettings");
			}
			else
			{
				var responseContent = await response.Content.ReadAsStringAsync();
				var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent);
				errorMessage = errorResponse?.Detail ?? "An error occurred while saving mode.";
			}
		}
		public async Task PutSelectedPlants()
        {

            if (!string.IsNullOrWhiteSpace(selectedPlant1) || !string.IsNullOrWhiteSpace(selectedPlant2))
            {
                // Perform username and password policy checks if they are being updated
                if (!string.IsNullOrEmpty(selectedPlant1))
                {
                    selectedPlants.SelectedPlant1 = selectedPlant1;
                }

                if (!string.IsNullOrEmpty(selectedPlant2))
                {
                    selectedPlants.SelectedPlant2 = selectedPlant2;
                }
                if (string.IsNullOrEmpty(selectedPlant1))
                {
                    selectedPlants.SelectedPlant1 = current1plant;
                }

                if (string.IsNullOrEmpty(selectedPlant2))
                {
                    selectedPlants.SelectedPlant2 = current2plant;
                }
                //make put request
                string json = System.Text.Json.JsonSerializer.Serialize(selectedPlants);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/settings/selectedplants/{settingsId}", content);

                if (response.IsSuccessStatusCode)
                {
                    message = "Updated plant";
                    await JS.InvokeVoidAsync("closeModal", "myModalSetupSensor");
                    Navigation.NavigateTo("/signup");
                    Navigation.NavigateTo("/");
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent);
                    errorMessage = errorResponse?.Detail ?? "An error occurred while saving plants.";
                }
            }

        }

   
        // --------------------------- Sensor --------------------------- //
        public async Task SetupSensor()
        {
            //make put request
            string json = System.Text.Json.JsonSerializer.Serialize(plantProfile);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/settings/{AccountSession.UserSession.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                message = "Updated name";
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent);
                errorMessage = errorResponse?.Detail ?? "An error occurred while saving settings.";
            }
        }

        // ---------------------------- Misc ---------------------------- //

        // The auto or manual mode toggle for the Arduino 
        //make a put request then auto changes a put request
        private async Task Toggle()
        {
            // Toggle the state
            mode.AutoMode = !mode.AutoMode;
        }

    }
}
