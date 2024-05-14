using API.Models;
using Microsoft.AspNetCore.Components;
using System.Security.Cryptography;
using System.Text;
namespace BlazorApp.Components.Pages

{
    public partial class Signup
    {
        User userSignup = new User();
        public string errorMessage = "";
        private HttpClient client = new HttpClient() { BaseAddress = new Uri("https://h3-projektv2-24q2h3-gruppe1-sqve.onrender.com") };

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

                if (response.IsSuccessStatusCode)
                {
                    // Registration successful
                    NavigationManager.NavigateTo("/login");
                }
                else
                {
                    // Registration failed
                    NavigationManager.NavigateTo("/signup");
                }
            }
        }
    }
}

