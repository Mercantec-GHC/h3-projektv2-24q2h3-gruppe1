using API.Migrations;
using API.Models;
using BlazorApp.Containers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Syncfusion.Blazor.Charts.Chart.Internal;
using System.Text;


namespace BlazorApp.Components.Layout
{
    public partial class MainLayout
    {
        string message = "";
        string errorMessage = "";
        string errorMessageLogin = "";
        string errorMessageSignup = "";
        string errorMessageEditProfile = "";

        bool usernameCheck = false;
        bool passwordCheck = false;
        private bool IsAutoChecked = true;
        private bool IsManualChecked = false;

        public List<Setting>? settingList;

        public UserLoginRequest userLogin = new UserLoginRequest();
        public UserSignUpRequest userSignup = new UserSignUpRequest();
      
        public UserPutRequest userProfile = new UserPutRequest();
        public Setting settings = new Setting();
        private HttpClient client = new HttpClient() { BaseAddress = new Uri("https://h3-projektv2-24q2h3-gruppe1-rolc.onrender.com") };

        // Sign up user WIP (Work in progress)
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
                    errorMessage = "Both username and password are invalid. Please try again.";
                }
                else if (!usernameCheck)
                {
                    errorMessage = "Username is invalid. Please try again.";
                }
                else
                {
                    errorMessage = "Password is invalid. Please try again.";
                }
            }
            else
            {
                string json = System.Text.Json.JsonSerializer.Serialize(userSignup);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Users", content);

                if (response.IsSuccessStatusCode)
                {
                    message = "Registration successful";

                    // Deserialize the created user from the response content
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var newUser = System.Text.Json.JsonSerializer.Deserialize<User>(responseContent);

                    // Create a setup settings for the new user
                    var settings = new Setting
                    {
                        UserId = newUser.Id,
                    };

                    string jsonPostSettings = System.Text.Json.JsonSerializer.Serialize(settings);
                    var contentPostSettings = new StringContent(jsonPostSettings, Encoding.UTF8, "application/json");
                    var responsePostSettings = await client.PostAsync("api/settings", contentPostSettings);

                    if (responsePostSettings.IsSuccessStatusCode)
                    {
                        await JS.InvokeVoidAsync("closeModal", "myModalSignup");
                    }
                    else
                    {
                        // Handle error when creating settings
                        var settingsResponseContent = await responsePostSettings.Content.ReadAsStringAsync();
                        var settingsErrorResponse = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(settingsResponseContent);
                        errorMessageSignup = settingsErrorResponse?.Detail ?? "An error occurred while creating settings.";
                    }
                }
                else
                {
                    // Read the response content to get the error message from the API
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent);
                    errorMessageSignup = errorResponse?.Detail ?? "An error occurred during signup.";
                }
            }
        }

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
               
                        Navigation.NavigateTo("/signup");
                        Navigation.NavigateTo("/");

                        await JS.InvokeVoidAsync("closeModal", "myModalLogin");

                        message = "Login successful";
                    }
                }

                else
                {
                    // Registration failed, navigate to signup page
                    errorMessageLogin = "Wrong username or password. Please try again.";
                }
            }
        }

        // Logout of account
        public void Logout()
        {
            try
            {
                AccountSession.UserSession = null;
                // Force reload
                Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
        // Edit profile
        public async Task HandleEditProfile()
        {
            if (!string.IsNullOrWhiteSpace(userProfile.Username) || !string.IsNullOrWhiteSpace(userProfile.Password))
            {
                // Perform username and password policy checks if they are being updated
                if (!string.IsNullOrEmpty(userProfile.Username))
                {
                    var usernameMessage = UsernamePolicyCheck(userProfile.Username);
                    if (usernameMessage != "Username is valid")
                    {
                        errorMessageEditProfile = usernameMessage;
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(userProfile.Password))
                {
                    var passwordMessage = PasswordPolicyCheck(userProfile.Password);
                    if (passwordMessage != "Password is valid")
                    {
                        errorMessageEditProfile = passwordMessage;
                        return;
                    }
                }

                // Serialize the user profile object
                string json = System.Text.Json.JsonSerializer.Serialize(userProfile);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/Users/{AccountSession.UserSession.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    message = "Profile updated successfully";
                    Navigation.NavigateTo("/");
                    await JS.InvokeVoidAsync("closeModal", "myModalEditProfile");
                }
                else
                {
                    // Read the response content to get the error message from the API
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent);
                    errorMessageEditProfile = errorResponse?.Detail ?? "An error occurred while updating the profile.";
                }
            }
            else
            {
                errorMessageEditProfile = "Please provide a username or password to update.";
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
