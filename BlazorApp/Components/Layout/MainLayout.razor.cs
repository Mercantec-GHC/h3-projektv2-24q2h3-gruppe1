using API.Models;
using BlazorApp.Containers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Charts.Chart.Internal;
using System.Text;


namespace BlazorApp.Components.Layout
{
    public partial class MainLayout
    {
        string message = "";
        string errorMessage = "";

  

        bool usernameCheck = false;
        bool passwordCheck = false;

     

        public List<Setting>? settingList;

        public UserLoginRequest userLogin = new UserLoginRequest();
        public UserSignUpRequest userSignup = new UserSignUpRequest();
        public User userProfile = new User();

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
                }
                else
                {
                    // Read the response content to get the error message from the API
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent);
                    errorMessage = errorResponse?.Detail;
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
                        // Force reload
                    
                        Navigation.NavigateTo("/");
                        await JS.InvokeVoidAsync("closeModal", "myModalLogin");

                        message = "Login successful";
                    }
                }

                else
                {
                    // Registration failed, navigate to signup page
                    errorMessage = "Registration failed. Please try again.";

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
