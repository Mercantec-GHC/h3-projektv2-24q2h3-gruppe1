using API.Models;
using BlazorApp.Services;
using BlazorApp.Containers;
using Microsoft.AspNetCore.Components;

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
            if (!string.IsNullOrWhiteSpace(userLogin.Username) || !string.IsNullOrWhiteSpace(userLogin.Email) && !string.IsNullOrWhiteSpace(userLogin.Password))
            {
                //Variables
                string email = "";
                string username = "Mads";
                string password = "Password3";


                UserService userService = new UserService();
                //User validUserInfo = await UserService.GetUserUserInfoAsync(username, email, password);

                //if (userLogin.UserInfo.Contains("@"))
                //{
                //    email = userLogin.UserInfo;
                //}
                //else
                //{
                //    username = userLogin.UserInfo;
                //}

                //if (validUserInfo != null)
                //{
                //    AccountSession.UserSession = validUserInfo;
                //    NavigationManager.NavigateTo("/home");
                //}

                //else
                //{
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
    }
}
