using static System.Net.WebRequestMethods;
using API.Models;
using Newtonsoft.Json;
using System.Text;
using System.Drawing.Printing;

namespace BlazorApp.Services
{
    public class UserService
    {
        //remember to change the call method to the specified CRUD operation you want to use
        string UserApi = "https://h3-projektv2-24q2h3-gruppe1-sqve.onrender.com/api/Users";

        public async Task<bool> PostUserAsync(User User)
        {
            string jsonData = JsonConvert.SerializeObject(User);

            HttpClient UserClient = new HttpClient();

            HttpResponseMessage response = new HttpResponseMessage();

            StringContent data = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                response = await UserClient.PostAsync(UserApi, data);

                return response.IsSuccessStatusCode;
            }

            catch (Exception)
            {
                return false;
            }
        }

        public async Task<User> GetUserUserInfoAsync(string username, string password)
        {
            try
            {
                // Create HttpClient instance (preferably reuse HttpClient instance)
                using (HttpClient userClient = new HttpClient())
                {
                    // Construct the API URL (avoid passing sensitive information in URL)
                    string userApi = $"https://h3-projektv2-24q2h3-gruppe1-sqve.onrender.com/api/Users/{username}/{password}";

                    // Make HTTP GET request
                    HttpResponseMessage response = await userClient.GetAsync(userApi);

                    // Check if request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read response content
                        string jsonData = await response.Content.ReadAsStringAsync();

                        // Deserialize JSON to User object
                        User user = JsonConvert.DeserializeObject<User>(jsonData);

                        // Validate user's password here if necessary
                        if (user.Username == username && user.Password == password)
                        {
                            return user;
                        }
                        //else if (user.Email == email && user.Password == password)
                        //{
                        //}
                    }
                    else
                    {
                        // Log unsuccessful response status code
                        Console.WriteLine($"HTTP request failed with status code: {response.StatusCode}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Log HTTP request exception
                Console.WriteLine($"HTTP request exception: {ex.Message}");
            }
            catch (JsonException ex)
            {
                // Log JSON deserialization exception
                Console.WriteLine($"JSON deserialization exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log other exceptions
                Console.WriteLine($"Unexpected exception: {ex.Message}");
            }

            return null;
        }
    }
}
