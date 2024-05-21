using API.Data;
using API.Models;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using API.Utilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDBContext _context;

        // Constructor to inject the database context
        public UsersController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            // Retrieve all users from the database
            return await _context.Users.ToListAsync();
        }

        // ---------------- User Credentials ------------------ //

        // POST: api/Users/login
        [HttpPost("login")] // Route for user login
        public async Task<ActionResult<User>> UserLogin(UserLoginRequest request)
        {
            var userFinder = await _context.Users.Where(item => item.Username == request.Username).ToListAsync();

            if (userFinder.Count == 0)
            {
                return BadRequest("Wrong username");
            }

            // Retrieve user information from the database
            var userFromDatabase = userFinder[0];

            // Compare the hashed password with the provided password
            var passwordIsSame = HashedPassword.FromHashAndSalt(userFromDatabase.Password, userFromDatabase.Salt).Compare(request.Password);
         
            if (!passwordIsSame)
            {
                return BadRequest("Wrong password");
            }

            return Ok(userFromDatabase);
            //throw new NotImplementedException("Login Auth");

        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> UserSignUp(UserSignUpRequest request)
        {
            var resemail = await _context.Users.Where(item => item.Email == request.Email).ToListAsync();

            var resusername = await _context.Users.Where(item => item.Username == request.Username).ToListAsync();


            if (resemail.Count > 0)
            {
                return Problem("email is being used");
            }

            if (resusername.Count > 0)
            {
                return Problem("Username is being used");
            }

            var hashedPassword = HashedPassword.FromPassword(request.Password);

            // Create a new user object with hashed password and salt
            User userSignUp = new User()
            {
                Email = request.Email,
                Username = request.Username,
                Password = hashedPassword.Hash,
                Salt = hashedPassword.Salt,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
       

            // Add the new user to the database
            _context.Users.Add(userSignUp);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = userSignUp.Id }, userSignUp);
        }

        // ----------------------- ID ------------------------- //

        // GET: api/Users/id
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            // Retrieve a user by their ID from the database
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            // It says it itself 
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // It says it itself 
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Users/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // It says it itself 
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Delete the user from the database if the id exists
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ---------------------- Mis ------------------------ //

        // Helper method to check if a user with a specific ID exists
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }

}