using API.Models;
using BlazorApp.Services;
using BlazorApp.Containers;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace BlazorApp.Pages
{
    public partial class Login : ComponentBase
    {
        // Top level variables
        public string connectionString;
        public string errorMessage = "";
        public User userLogin = new User();
        public HttpClient client = new HttpClient();

        private async Task HandleLogin()
        {
            if (!string.IsNullOrWhiteSpace(userLogin.UserInfo) && !string.IsNullOrWhiteSpace(userLogin.Password))
            {
                // Variables
                string email = "";
                string username = "";
                string password = userLogin.Password;
                connectionString = Configuration.GetConnectionString("DefaultConnection");

                // Use HttpClient to send a GET request to your Swagger API
                var response = await client.GetAsync("api/Users");

                if (userLogin.UserInfo.Contains("@"))
                {
                    email = userLogin.UserInfo;
                }
                else
                {
                    username = userLogin.UserInfo;
                }

                UserService UserService = new UserService();
                User validUserInfo = await UserService.GetUserUserInfoAsync(username, email, password);

                if (validUserInfo != null)
                {
                    AccountSession.UserSession = validUserInfo;
                    NavigationManager.NavigateTo("/home");
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
