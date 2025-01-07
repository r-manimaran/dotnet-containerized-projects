using Microsoft.EntityFrameworkCore;

namespace OrderApi.Data;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{

}
