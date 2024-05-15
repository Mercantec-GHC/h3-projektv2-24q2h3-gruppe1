using API.Models;
using BlazorApp.Containers;
using BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Cryptography;
using System.Text;

namespace BlazorApp.Components.Pages
{
    public partial class Home
    {
        // Top level variables
        string connectionString;
        
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
        public List<Setting>? settingList;

        public Login userLogin = new ();
        public User userSignup = new User();
        public User userProfile = new User();

        public Plant plantProfile = new Plant();

        public bool IsAutoChecked = true;
        public bool IsManualChecked = false;
        
        private HttpClient client = new HttpClient() { BaseAddress = new Uri("https://h3-projektv2-24q2h3-gruppe1-rolc.onrender.com") };

        // Signup user
        public async Task HandleSignUp()
        {
            // Reset error message
            errorMessage = "";

            // Perform username and password policy checks
            UsernamePolicyCheck(userSignup.Username);
            PasswordPolicyCheck(userSignup.Password);

            // Check if username and password meet requirements
            if (!usernameCheck || !passwordCheck)
            {
                // If either username or password fails policy checks, set appropriate error message
                if (!usernameCheck && !passwordCheck)
                {
                    errorMessage = "Both credentials are invalid. Please try again.";
                }
                else if (!usernameCheck)
                {
                    errorMessage = "Username credentials are invalid. Please try again.";
                }
                else
                {
                    errorMessage = "Password credentials are invalid. Please try again.";
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
                        errorMessage = "Registration failed. Please try again.";
                    }
                }
            }


        // Login user 
        public async Task HandleLogin()
        {
            if (!string.IsNullOrWhiteSpace(userLogin.username) && !string.IsNullOrWhiteSpace(userLogin.password))
            {
                string json = System.Text.Json.JsonSerializer.Serialize(userLogin);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Users", content);

                if (response.IsSuccessStatusCode)
                {
                    message = "Registration succesfull";
                }
                else
                {
                    // Registration failed, navigate to signup page
                    errorMessage = "Registration failed. Please try again.";
                }
            }
        }


        public async Task HandleEditProfile()
        {
            if (!string.IsNullOrWhiteSpace(userProfile.Username) && !string.IsNullOrWhiteSpace(userProfile.Password))
            {

                if (userProfile.Username.Contains("@"))
                {
                    newEmail = userProfile.Email;
                }
                
                else
                {
                    newUsername = userProfile.Username;
                }

                // --------------------------------------- //

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
                // Handle empty input
                errorMessage = "Please enter a correct username and password. Note that both fields may be case-sensitive";
            }
        }


        // Edit profile WIP (Work in progress)

        // Create plant to database
        public async Task HandleCreatePlant()
        {
            string PlantName = "";
            int MinWaterLevel = 0;
            int MaxWaterLevel = 0;


            if (string.IsNullOrWhiteSpace(PlantName))
            {
                errorMessage = "invalid input try again";
            }

            if (!PlantName.All(char.IsLetterOrDigit))
            {
                errorMessage = "invalid input try again";
            }

            if(MinWaterLevel < 0 || MinWaterLevel > 100){
                errorMessage = "invalid input try again";
            }

            if (MaxWaterLevel < 0 || MaxWaterLevel > 100)
            {
                errorMessage = "invalid input try again";
            }

            else
            {
                message = "Plant created";
            }
        }

        // Edit plant info for the database
        public async Task HandleEditPlant()
        {
            string PlantName = "";
            int MinWaterLevel = 0;
            int MaxWaterLevel = 0;

            if (string.IsNullOrWhiteSpace(PlantName))
            {
                errorMessage = "invalid input try again";
            }

            if (!PlantName.All(char.IsLetterOrDigit))
            {
                errorMessage = "invalid input try again";
            }

            if (MinWaterLevel < 0 || MinWaterLevel > 100)
            {
                errorMessage = "invalid input try again";
            }

            if (MaxWaterLevel < 0 || MaxWaterLevel > 100)
            {
                errorMessage = "invalid input try again";
            }

            else
            {
                message = "Plant created";
            }
        }
        
        // --------------------------- //

        // Get the list of plants for drop down menu
        public async Task GetListOfPlants()
        {
            // It takes the list of plants we have in our API via the endpoint "api/Plants"
            try
            {
                plants = await client.GetFromJsonAsync<List<Plant>>("api/Plants");

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

        public async Task SetupSensor()
        {

        }

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

        // This is an email policy for insuring that there is fx. @ so that we are sure that it is a valid email 
        public void EmailPolicyCheck(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                errorMessage = "Email cannot be empty or contain only whitespace!";
            }

            if (!email.All(char.IsLetterOrDigit))
            {
                errorMessage = "Only letters and digits are allowed in the email!";
            }

            if (!email.Contains("@"))
            {
                errorMessage = "Email is invalid";
            }

            else
            {
                message = "Email is accepted!";
            }
        }


        // This is a username policy for insuring that this isnt fx. @ so we can differenciate between mail and username 
        public string UsernamePolicyCheck(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return "Username cannot be empty or contain only whitespace!";
            }

            if (username.Length < 2)
            {
                return "Username must be at least 2 characters!";
            }

            if (!username.All(char.IsLetterOrDigit))
            {
                return "Only letters and digits are allowed in the username!";
            }

            if (!username.Any(char.IsUpper))
            {
                return "Username must contain uppercase letters!";
            }

            if (!username.Any(char.IsLower))
            {
                return "Username must contain lowercase letters!";
            }

            else
            {
                usernameCheck = true;
                return "Username is valid";
            }
        }

        public string PasswordPolicyCheck(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return "Password cannot be empty or contain only whitespace!";
            }

            if (password.Length < 5)
            {
                return "Password must be at least 5 characters!";
            }

            if (!password.Any(char.IsUpper))
            {
                return "Password must contain uppercase letters!";
            }

            if (!password.Any(char.IsLower))
            {
                return "Password must contain lowercase letters!";
            }

            if (!password.Any(char.IsDigit))
            {
                return "Password must contain numbers!";
            }

            else
            {
                passwordCheck = true;
                return "Password is valid";
            }
        }
    }
}
