using static System.Net.WebRequestMethods;
using API.Models;
using Newtonsoft.Json;
using System.Text;

namespace BlazorApp.Services
{
    public class UserService
    {
        //remember to change the call method to the specified CRUD operation you want to use
        string UserApi = "https://localhost:7036/api/Users";

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

        public async Task<User> GetUserUserInfoAsync(string username, string email, string password)
        {
            HttpClient UserClient = new HttpClient();

            UserApi = $"https://h3-projektv2-24q2h3-gruppe1-sqve.onrender.com/api/Users/{username}/{email}/{password}"; // Change the API string to match our API

            HttpResponseMessage response = new HttpResponseMessage();

            User User = new User();

            string jsonData = "";

            try
            {
                response = await UserClient.GetAsync(UserApi);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                jsonData = await response.Content.ReadAsStringAsync();
                User = JsonConvert.DeserializeObject<User>(jsonData);
                if (User != null)
                {
                    return User;
                }
            }

            catch (Exception)
            {
                return null;
            }

            return null;
        }
    }
}
