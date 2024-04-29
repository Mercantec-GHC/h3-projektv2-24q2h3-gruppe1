using API.Models;
using BlazorApp.Services;
using BlazorApp.Containers;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages
{
    public partial class Login
    {
        public User userLogin = new User();
        public string errorMessage = "";

        private async Task HandleLogin()
        {
            if (!string.IsNullOrWhiteSpace(userLogin.Username) && !string.IsNullOrWhiteSpace(userLogin.Password))
            {
                string email = userLogin.Email;
                string username = userLogin.Username;
                string password = userLogin.Password;

                UserService UserService = new UserService();
                User validUserEmail = await UserService.GetUserEmailAsync(email, password);
                //User validUserUsername = await UserService.GetUserUsernameAsync(username, password);

                if (validUserEmail != null)
                {
                    AccountSession.UserSession = validUserEmail;
                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    errorMessage = "Invalid credentials. Please check your username/ email and password.";
                }
            }
            else
            {
                // Handle empty input
                errorMessage = "Please enter your username/ email and password.";
                StateHasChanged();
            }
        }

        public void Logout()
        {
            try
            {
                AccountSession.UserSession = null;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
