using Microsoft.EntityFrameworkCore;

namespace RestaurantSystem.Infrastructure
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

    }
}
