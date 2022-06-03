using Microsoft.EntityFrameworkCore;
using MyDataCoin.Entities;

namespace MyDataCoin.DataAccess
{
    public class WebApiDbContext : DbContext
    {
        public WebApiDbContext(DbContextOptions<WebApiDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Email> Emails { get; set; }

        public DbSet<EmailCodeDictionary> EmailCodeDictionaries { get; set; }

        public virtual DbSet<UserRefreshTokens> UserRefreshToken { get; set; }
    }
}
