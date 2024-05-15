using API.Data;
using API.Models;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

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

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _context.Users.ToListAsync();
        }

        // ---------------- User Credentials ------------------ //

        // POST: api/Users/username/password
        [HttpPost]
        public async Task<ActionResult<User>> UserLogin(UserLoginRequest request)
        {
            User userLogin = new User();

            userLogin.Username = request.Username;
            userLogin.Password = request.Password;


            // Generate a random salt
            string salt = userLogin.Salt;

            using (var sha256 = new SHA256Managed())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(request.Password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                //  Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

                // Assuming you might want to update the password with the hash
                request.Password = Convert.ToBase64String(hashedBytes);
            }

            var userFinder = await _context.Users.Where(item => item.Username == request.Username && item.Password == request.Password).ToListAsync();

            return userFinder == null || userFinder.Count() != 1 ? NotFound() : userFinder.First();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> UserSignUp(UserSignUpRequest request)
        {
            User userSignUp = new();

            userSignUp.Username = request.Username;
            userSignUp.Password = request.Password;

            // Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            using (var sha256 = new SHA256Managed())
            {

                userSignUp.Salt = salt.ToString();
                byte[] passwordBytes = Encoding.UTF8.GetBytes(request.Password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

                // Assuming you might want to update the password with the hash
                request.Password = Convert.ToBase64String(hashedBytes);
            }
            // is not inf
            userSignUp.UpdatedAt = DateTime.UtcNow;
            userSignUp.CreatedAt = DateTime.UtcNow;
            _context.Users.Add(userSignUp);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = userSignUp.Id }, request);
        }

        // ----------------------- ID ------------------------- //

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

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}