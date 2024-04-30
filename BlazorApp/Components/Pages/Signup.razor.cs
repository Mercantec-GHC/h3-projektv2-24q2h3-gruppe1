using API.Models;
using BlazorApp.Services;
using BlazorApp.Containers;
using Microsoft.AspNetCore.Components;
using System.Security.Cryptography;
using System.Text;
namespace BlazorApp.Components.Pages
{
    public partial class Signup
    {
        private HttpClient client = new HttpClient() { BaseAddress = new Uri("https://h3-projektv2-24q2h3-gruppe1-sqve.onrender.com") };
        User userSignup = new User();
        public async Task HandleSignUp()
        {
            //hash the password
            var sha = SHA256.Create();
            var passwordBytes = Encoding.Default.GetBytes(userSignup.Password);
            var hashedPasswordBytes = sha.ComputeHash(passwordBytes);
            userSignup.Password = BitConverter.ToString(hashedPasswordBytes).Replace("-", "").ToLower();

            // Serialize the userModel to JSON
            string json = System.Text.Json.JsonSerializer.Serialize(userSignup);

            // Create HttpContent with JSON data
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Use HttpClient to send a POST request to your Swagger API
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

