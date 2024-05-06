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
        public User userLogin = new User();
        public User userSignup = new User();
        private HttpClient client = new HttpClient() { BaseAddress = new Uri("https://h3-projektv2-24q2h3-gruppe1-sqve.onrender.com") };

        private async Task HandleLogin()
        {
            if (!string.IsNullOrWhiteSpace(userLogin.Username) || !string.IsNullOrWhiteSpace(userLogin.Email) && !string.IsNullOrWhiteSpace(userLogin.Password))
            {
                // Variables
                string email = "";
                string username = "";
                string password = "";

                // Assuming UserService has a method like GetUserAsync for fetching user info
                UserService userService = new UserService(); // Instantiate your UserService
                User validUserInfo = await userService.GetUserUserInfoAsync(username, email, password);

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

        async Task HashPassword()
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            using (var sha256 = new SHA256Managed())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(userSignup.Password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

                // Assuming you might want to update the password with the hash
                userSignup.Password = Convert.ToBase64String(hashedBytes);

                string json = System.Text.Json.JsonSerializer.Serialize(userSignup);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Users", content);
            }
        }

        private void PasswordPolicyCheck(string password)
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

        private void UsernamePolicyCheck(string username)
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

        private void EmailPolicyCheck(string email)
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

        private bool IsAutoChecked = true;
        private bool IsManualChecked = false;

        private void Toggle(char switchName)
        {
            if (!IsManualChecked && !IsAutoChecked)
            {
                IsAutoChecked = true;
            }
            else
            {
                IsAutoChecked = !IsAutoChecked;
                IsManualChecked = !IsManualChecked;
            }
        }
    }
}
