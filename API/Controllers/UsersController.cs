using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDBContext _context;
  
        public UsersController(AppDBContext context)
        {
            _context = context;
        }

        // --------------------------------------------------- //

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.Users.ToListAsync();
        }

        // --------------------------------------------------- //

        // Post: api/Users/login
        [HttpPost]
        public async Task<ActionResult<User>> UserLogin(UserLoginRequest userLoginRequest)
        {
            User user = new User();

            user.Username = userLoginRequest.Username;
            user.Password = userLoginRequest.Password;

            // Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            using (var sha256 = new SHA256Managed())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(userLoginRequest.Password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

                // Assuming you might want to update the password with the hash
                userLoginRequest.Password = Convert.ToBase64String(hashedBytes);
            }
            
            var userFinder = await _context.Users.Where(item => item.Username == userLoginRequest.Username && item.Password == userLoginRequest.Password).ToListAsync();

            return user == null || userFinder.Count() != 1 ? NotFound() : userFinder.First();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> UserSignUp(UserSignUpRequest userSignUpRequest)
        {
            User user = new User();

            // Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }


            using (var sha256 = new SHA256Managed())
            {
                user.Username = userSignUpRequest.Username;
                user.Password = userSignUpRequest.Password;

                user.Salt = salt.ToString();
                //salt = user.Salt;
                byte[] passwordBytes = Encoding.UTF8.GetBytes(user.Password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

                // Assuming you might want to update the password with the hash
                userSignUpRequest.Password = Convert.ToBase64String(hashedBytes);
            }
            // is not inf
            user.UpdatedAt = DateTime.UtcNow;
            user.CreatedAt = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // --------------------------------------------------- //

        // GET: api/Users/id
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
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
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Check if user exists
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
