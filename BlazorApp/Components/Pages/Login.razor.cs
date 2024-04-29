using API.Models;

namespace BlazorApp.Components.Pages
{
    public partial class Login
    {
        public User userLogin = new User();
        public string errorMessage = "";

        private async Task HandleLogin()
        {
            if (!string.IsNullOrWhiteSpace(userLogin.username) && !string.IsNullOrWhiteSpace(userLogin.password))
            {
                string email = userLogin.email;
                string username = userLogin.username;
                string password = userLogin.password;

                UserService UserService = new UserService();
                User validCustomer = await UserService.GetCustomerEmailAsync(email, password);

                if (validCustomer != null)
                {
                    AccountSession.CustomerSession = validCustomer;
                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    AdminService adminService = new AdminService();
                    Admin validAdmin = await adminService.GetAdminEmailAsync(email, password);
                    if (validAdmin != null)
                    {
                        AccountSession.AdminSession = validAdmin;
                        NavigationManager.NavigateTo("/");
                    }
                    errorMessage = "Invalid credentials. Please check your username/ email and password.";
                    StateHasChanged();
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
                AccountSession.CustomerSession = null;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
