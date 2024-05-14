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

        string userApi = "https://h3-projektv2-24q2h3-gruppe1-sqve.onrender.com/api/Users";

        // Method to retrieve user data from the API
        public async Task<User> GetUserAsync(string username, string password)
        {
            HttpClient client = new HttpClient();

            try
            {
                // Construct the API endpoint with username and password parameters
                string apiEndpoint = $"{userApi}/login";

                // Send GET request to the API
                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                // Check if request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read and deserialize the response content
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    User user = JsonConvert.DeserializeObject<User>(jsonResponse);
                    return user;
                }
                else
                {
                    // Request failed, return null or handle error as needed
                    return null;
                }
            }
            catch (Exception)
            {
                // Exception occurred, return null or handle error as needed
                return null;
            }
        }
    }
}
