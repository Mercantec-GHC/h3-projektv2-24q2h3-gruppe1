using API.Models;
using BlazorApp.Containers;
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

        string email = "";
        string username = "";
        string password = "";

        string newEmail = "";
        string newUsername = "";
        string newPassword = "";

        bool usernameCheck = false;
        bool passwordCheck = false;

        public List<Plant>? plants;
        public List<Plant>? Useronlyplants;

        public List<Setting>? settingList;

        public UserLoginRequest userLogin = new UserLoginRequest();
        public UserSignUpRequest userSignup = new UserSignUpRequest();
        public User userProfile = new User();

        public Plant plantProfile = new Plant();

        public bool IsAutoChecked = true;
        public bool IsManualChecked = false;

        private HttpClient client = new HttpClient() { BaseAddress = new Uri("https://h3-projektv2-24q2h3-gruppe1-rolc.onrender.com") };
        #endregion

        // --------------------------- Users ---------------------------- //

        public async Task HandleLogin()
        {
            if (!string.IsNullOrWhiteSpace(userLogin.Username) && !string.IsNullOrWhiteSpace(userLogin.Password))
            {
                string json = System.Text.Json.JsonSerializer.Serialize(userLogin);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Users/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response Content: " + responseContent); // Debug: Log the response content

                    var user = System.Text.Json.JsonSerializer.Deserialize<User>(responseContent);

                    if (user != null)
                    {
                        Console.WriteLine("Deserialized Id: " + user.Id); // Debug: Log the deserialized Id
                        
                        AccountSession.UserSession = user;

                        message = "Login successful";
                        NavigationManager.NavigateTo("/");
                    }
                }

                else
                {
                    // Registration failed, navigate to signup page
                    errorMessage = "Registration failed. Please try again.";

                }
            }
        }

        // Edit profile WIP (Work in progress)
        public async Task HandleEditProfile()
        {
            if (!string.IsNullOrWhiteSpace(userProfile.Username) && !string.IsNullOrWhiteSpace(userProfile.Password))
            {
                //make put requests on user model

                if (userProfile.Username.Contains("@"))
                {
                    newEmail = userProfile.Email;
                }

                else
                {
                    newUsername = userProfile.Username;
                }

                if (userProfile.Username != newEmail || userProfile.Username != newUsername)
                {
                    errorMessage = "Invalid credentials. Please make sure you have a different email or username";
                }

                if (userProfile.Password != newPassword)
                {
                    errorMessage = "Invalid credentials. Please make sure you have a different password";
                }
            }
            else
            {
                string json = System.Text.Json.JsonSerializer.Serialize(userSignup);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Users", content);


                if (response.IsSuccessStatusCode)
                {
                    message = "Registration succesfull";
                }

                else
                {
                    // Registration failed, navigate to signup page
                    errorMessage = "Please enter a correct username and password. Note that both fields may be case-sensitive";
                }
            }
        }

        // -------------------------- Plants ---------------------------- //

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
                    errorMessage = "select a plant "; 
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
                    NavigationManager.NavigateTo("/");
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
                settingList = await client.GetFromJsonAsync<List<Setting>>("api/Settings");
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error fetching Settings: {ex.Message}");
            }
        }


        // --------------------------- Sensor --------------------------- //
        public async Task SetupSensor()
        {

        }

        // ---------------------------- Misc ---------------------------- //

        // Logout of account
        public void Logout()
        {
            try
            {
                AccountSession.UserSession = null;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        // The auto or manual mode toggle for the Arduino 
        //make a put request then auto changes a put request
        public void Toggle(char switchName)
        {
            // It checks if both toggles are set to false and sets IsAutoChecked to true 
            if (!IsManualChecked && !IsAutoChecked)
            {
                IsAutoChecked = true;
                // setting.AutoMode = true;
            }
            // The else insures that no matter what if both are set to false that the if statement is true insuring one is always active
            else
            {
                IsAutoChecked = !IsAutoChecked;
                IsManualChecked = !IsManualChecked;
                //setting.AutoMode = false;
            }
        }
    }
}
