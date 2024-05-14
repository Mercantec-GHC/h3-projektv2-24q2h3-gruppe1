using API.Models;
using BlazorApp.Services;
using BlazorApp.Containers;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace BlazorApp.Pages
{
    public partial class Login : ComponentBase
    {
        // Top level variables
        public string connectionString;
        public string errorMessage = "";
        public User userLogin = new User();
        private HttpClient client = new HttpClient() { BaseAddress = new Uri("https://h3-projektv2-24q2h3-gruppe1-sqve.onrender.com") };

        private async Task HandleLogin()
        {
            if (!string.IsNullOrWhiteSpace(userLogin.Username) && !string.IsNullOrWhiteSpace(userLogin.Password))
            {

              //  User validUserInfo = await UserService.GetUserInfoAsync(userLogin.Username, userLogin.Password);

                //if (validUserInfo != null)
                //{
                //    // User authenticated successfully
                //    postDefaultSettings();
                //    AccountSession.UserSession = validUserInfo;
                //    NavigationManager.NavigateTo("/home");
                //}
                //else
                //{
                //    // Authentication failed
                //    errorMessage = "Invalid credentials. Please check your username and password.";
                //}
            }
            else
            {
                // Handle empty input
                errorMessage = "Please enter a correct username and password. Note that both fields may be case-sensitive";
            }
        }

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


        async Task postDefaultSettings()
        {
            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(userLogin.Id);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/settings", content);
            }
            catch (Exception ex) { }
        }
    }
}
