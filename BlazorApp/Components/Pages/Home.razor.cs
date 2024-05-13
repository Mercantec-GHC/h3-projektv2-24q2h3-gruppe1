using API.Models;
using System.Text;
using BlazorApp.Services;
using BlazorApp.Containers;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Components.Pages
{
    public partial class Home
    {
        // Top level variables
        public string connectionString;
        public string errorMessage = "";
        
        public List<Plant>? plants;
        public List<Setting>? settingList;

        public User userLogin = new User();
        public User userSignup = new User();
        public User userProfile = new User();
        
        public Plant plantProfile = new Plant();


        public bool IsAutoChecked = true;
        public bool IsManualChecked = false;
        
        public HttpClient client = new HttpClient() { BaseAddress = new Uri("https://h3-projektv2-24q2h3-gruppe1-sqve.onrender.com") };

        public async Task HandleLogin()
        {
            if (!string.IsNullOrWhiteSpace(userLogin.Username) && !string.IsNullOrWhiteSpace(userLogin.Password))
            {
                // Variables
                string email = "";
                string username = userLogin.Username;
                string password = userLogin.Password;


                User validUserInfo = await UserService.GetUserInfoAsync(username, password);

                if (userLogin.Username.Contains("@"))
                {
                    email = userLogin.Username;
                }
                else
                {
                    username = userLogin.Username;
                }

                if (validUserInfo != null)
                {
                    // Navigate to the home page after successful login
                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    errorMessage = "Invalid credentials. Please check your username and password.";
                }
            }
            else
            {
                // Handle empty input
                errorMessage = "Please enter a correct username and password. Note that both fields may be case-sensitive";
            }
        }

        async Task signup()
        {
            string json = System.Text.Json.JsonSerializer.Serialize(userSignup);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Users", content);
            if (response.IsSuccessStatusCode)
            {
                // Registration successful
                NavigationManager.NavigateTo("/login");
            }
        }

        public async Task HandleEditProfile()
        {
            string plantProfile = "";

        }

        public async Task HandleCreatePlant()
        {
            string plantProfile = "";
        }

        public async Task HandleEditPlant()
        {

        }

        // 
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

        // 
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
        
        //
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
                errorMessage = "Email is accepted!";
            }
        }

        // This is a username policy for insuring that this isnt fx. @ so we can differenciate between mail and username 
        public void UsernamePolicyCheck(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "Username cannot be empty or contain only whitespace!";
            }

            if (username.Length < 8)
            {
                errorMessage = "Username must be at least 8 characters!";
            }

            if (!username.All(char.IsLetterOrDigit))
            {
                errorMessage = "Only letters and digits are allowed in the username!";
            }

            if (!username.Any(char.IsUpper))
            {
                errorMessage = "Username must contain uppercase letters!";
            }

            if (!username.Any(char.IsLower))
            {
                errorMessage = "Username must contain lowercase letters!";
            }

            if (!username.Any(char.IsDigit))
            {
                errorMessage = "Username must contain numbers!";
            }

            if (username.Any(char.IsSymbol))
            {
                errorMessage = "Username cant contain special characters";
            }

            else
            {
                errorMessage = "Username is accepted!";
            }
        }

        // This is a password policy for insuring that the password is safer then fx. merc1234
        public void PasswordPolicyCheck(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Password cannot be empty or contain only whitespace!";
            }

            if (password.Length < 10)
            {
                errorMessage = "Password must be at least 16 characters!";
            }

            if (!password.Any(char.IsUpper))
            {
                errorMessage = "Password must contain uppercase letters!";
            }

            if (!password.Any(char.IsLower))
            {
                errorMessage = "Password must contain lowercase letters!";
            }

            if (!password.Any(char.IsDigit))
            {
                errorMessage = "Password must contain numbers!";
            }

            if (!password.Any(c => char.IsSymbol(c) || char.IsPunctuation(c)))
            {
                errorMessage = "Password must contain special characters!";
            }

            else
            {
                errorMessage = "Password is accepted!";
            }
        }

    }
}
