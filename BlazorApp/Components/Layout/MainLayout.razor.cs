namespace BlazorApp.Components.Layout
{
    public class MainLayout
    {
		#region Top Level Variables
		string message = "";
		string errorMessage = "";

		string email = "";
		string username = "";
		string password = "";

		string newEmail = "";
		string newUsername = "";
		string newPassword = "";

		bool usernameCheck = false;
		bool passwordCheck = false;

		public List<Plant>? plants;
		public List<Setting>? settingList;

		public UserLoginRequest userLogin = new UserLoginRequest();
		public UserSignUpRequest userSignup = new UserSignUpRequest();
		public User userProfile = new User();

		public Plant plantProfile = new Plant();

		public bool IsAutoChecked = true;
		public bool IsManualChecked = false;

		private HttpClient client = new HttpClient() { BaseAddress = new Uri("https://h3-projektv2-24q2h3-gruppe1-rolc.onrender.com") };
		#endregion

		// --------------------------- Users ---------------------------- //

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
					errorMessage = "Both username and password are invalid. Please try again.";
				}
				else if (!usernameCheck)
				{
					errorMessage = "Username is invalid. Please try again.";
				}
				else
				{
					errorMessage = "Password is invalid. Please try again.";
				}
			}
			else
			{
				string json = System.Text.Json.JsonSerializer.Serialize(userSignup);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await client.PostAsync("api/Users", content);

				if (response.IsSuccessStatusCode)
				{
					message = "Registration successful";
				}
				else
				{
					// Read the response content to get the error message from the API
					var responseContent = await response.Content.ReadAsStringAsync();
					var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent);
					errorMessage = errorResponse?.Detail;
				}
			}


		}

		public async Task HandleLogin()
		{
			if (!string.IsNullOrWhiteSpace(userLogin.Username) && !string.IsNullOrWhiteSpace(userLogin.Password))
			{
				string json = System.Text.Json.JsonSerializer.Serialize(userLogin);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await client.PostAsync("api/Users/login", content);

				if (response.IsSuccessStatusCode)
				{
					var responseContent = await response.Content.ReadAsStringAsync();
					Console.WriteLine("Response Content: " + responseContent); // Debug: Log the response content

					var user = System.Text.Json.JsonSerializer.Deserialize<User>(responseContent);

					if (user != null)
					{
						Console.WriteLine("Deserialized Id: " + user.Id); // Debug: Log the deserialized Id

						AccountSession.UserSession = user;

						message = "Login successful";
						NavigationManager.NavigateTo("/");
					}
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
				//make put requests on user model

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
	}
}
