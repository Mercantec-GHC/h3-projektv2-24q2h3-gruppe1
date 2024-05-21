using API.Models;
using BlazorApp.Containers;
using BlazorBootstrap;
using System.Text;

namespace BlazorApp.Components.Layout
{
    public partial class MainLayout
    {
        #region Top Level Variables
        string connectionString;

        // Modal Message Variables
        string message = "";
        string errorMessage = "";

        // Modal User Input Variables
        string email = "";
        string username = "";
        string password = "";

        string newEmail = "";
        string newUsername = "";
        string newPassword = "";

        bool usernameCheck = false;
        bool passwordCheck = false;

        // Pie Variables
        public string[]? backgroundColors;
        public PieChart pieChart = default!;
        public PieChartOptions pieChartOptions = default!;
        public BlazorBootstrap.ChartData chartData = default!;

        public int datasetsCount = 0;
        public int dataLabelsCount = 0;

        // Random Variable
        public Random random = new();

        // Plants Variables
        public List<Plant>? plants;
        public List<Setting>? settingList;
        public Plant plantProfile = new Plant();

        public bool IsAutoChecked = true;
        public bool IsManualChecked = false;

        // User Variables
        public User userProfile = new User();
        public UserLoginRequest userLogin = new UserLoginRequest();
        public UserSignUpRequest userSignup = new UserSignUpRequest();

        // Connection
        private HttpClient client = new HttpClient() { BaseAddress = new Uri("https://h3-projektv2-24q2h3-gruppe1-rolc.onrender.com") };
        #endregion

        // --------------------------- Users ---------------------------- //
        #region User Sign Up, Login & Edit Profile
        // Sign up user WIP (Work in progress)
        public async Task HandleSignUp()
        {
            // Reset error message
            errorMessage = "";

            // Perform username and password policy checks
            UsernamePolicyCheck(userSignup.Username);
            PasswordPolicyCheck(userSignup.Password);

            // Check if username and password meet requirements
            if (!usernameCheck || !passwordCheck)
            {
                // If either username or password fails policy checks, set appropriate error message
                if (!usernameCheck && !passwordCheck)
                {
                    errorMessage = "Both credentials are invalid. Please try again.";
                }
                else if (!usernameCheck)
                {
                    errorMessage = "Username credentials are invalid. Please try again.";
                }
                else
                {
                    errorMessage = "Password credentials are invalid. Please try again.";
                }
            }

            else
            {
                string json = System.Text.Json.JsonSerializer.Serialize(userSignup);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Users", content);

                if (response.IsSuccessStatusCode)
                {
                    message = "Registration succesfull";
                }

                else
                {
                    // Registration failed, navigate to signup page
                    errorMessage = "Registration failed. Please try again.";
                }
            }
        }

        // Login user WIP (Work in progress)
        public async Task HandleLogin()
        {
            if (!string.IsNullOrWhiteSpace(userLogin.Username) && !string.IsNullOrWhiteSpace(userLogin.Password))
            {
                string json = System.Text.Json.JsonSerializer.Serialize(userLogin);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Users/login", content);

                if (response.IsSuccessStatusCode)
                {
                    message = "Registration succesfull";
                }

                else
                {
                    // Registration failed, navigate to signup page
                    errorMessage = "Registration failed. Please try again.";
                }
            }
        }

        // Edit profile WIP (Work in progress)
        public async Task HandleEditProfile()
        {
            if (!string.IsNullOrWhiteSpace(userProfile.Username) && !string.IsNullOrWhiteSpace(userProfile.Password))
            {

                if (userProfile.Username.Contains("@"))
                {
                    newEmail = userProfile.Email;
                }

                else
                {
                    newUsername = userProfile.Username;
                }

                if (userProfile.Username != newEmail || userProfile.Username != newUsername)
                {
                    errorMessage = "Invalid credentials. Please make sure you have a different email or username";
                }

                if (userProfile.Password != newPassword)
                {
                    errorMessage = "Invalid credentials. Please make sure you have a different password";
                }
            }
            else
            {
                string json = System.Text.Json.JsonSerializer.Serialize(userSignup);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Users", content);


                if (response.IsSuccessStatusCode)
                {
                    message = "Registration succesfull";
                }

                else
                {
                    // Registration failed, navigate to signup page
                    errorMessage = "Please enter a correct username and password. Note that both fields may be case-sensitive";
                }
            }
        }
        #endregion

        // ---------------------------- Misc ---------------------------- //
        #region Logout & Toggle Modes
        // Logout of account
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

        // The auto or manual mode toggle for the Arduino 
        public void Toggle(char switchName)
        {
            // It checks if both toggles are set to false and sets IsAutoChecked to true 
            if (!IsManualChecked && !IsAutoChecked)
            {
                IsAutoChecked = true;
                // setting.AutoMode = true;
            }
            // The else insures that no matter what if both are set to false that the if statement is true insuring one is always active
            else
            {
                IsAutoChecked = !IsAutoChecked;
                IsManualChecked = !IsManualChecked;
                //setting.AutoMode = false;
            }
        }
        #endregion

        // ---------------------------- Policy -------------------------- //
        #region Policies for sign up
        // This is an email policy for insuring that there is fx. @ so that we are sure that it is a valid email 
        public void EmailPolicyCheck(string email)
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
                message = "Email is accepted!";
            }
        }

        // This is a username policy for insuring that this isnt fx. @ so we can differenciate between mail and username 
        public string UsernamePolicyCheck(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return "Username cannot be empty or contain only whitespace!";
            }

            if (username.Length < 2)
            {
                return "Username must be at least 2 characters!";
            }

            if (!username.All(char.IsLetterOrDigit))
            {
                return "Only letters and digits are allowed in the username!";
            }

            if (!username.Any(char.IsUpper))
            {
                return "Username must contain uppercase letters!";
            }

            if (!username.Any(char.IsLower))
            {
                return "Username must contain lowercase letters!";
            }

            else
            {
                usernameCheck = true;
                return "Username is valid";
            }
        }

        public string PasswordPolicyCheck(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return "Password cannot be empty or contain only whitespace!";
            }

            if (password.Length < 5)
            {
                return "Password must be at least 5 characters!";
            }

            if (!password.Any(char.IsUpper))
            {
                return "Password must contain uppercase letters!";
            }

            if (!password.Any(char.IsLower))
            {
                return "Password must contain lowercase letters!";
            }

            if (!password.Any(char.IsDigit))
            {
                return "Password must contain numbers!";
            }

            else
            {
                passwordCheck = true;
                return "Password is valid";
            }
        }
        #endregion
    }
}
