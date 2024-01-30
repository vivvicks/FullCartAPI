using FullCartApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FullCartApi.Repositories
{
    public class UserRepository 
    {
        private readonly EcommerceDbContext _context;

        public UserRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public User GetUserByUsernameAndPassword(string username, string password)
        {
            // Hash the incoming password for comparison
            var hashedPassword = HashPassword(password);

            return _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Username == username && u.PasswordHash == hashedPassword);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }

}
