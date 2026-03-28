using Microsoft.EntityFrameworkCore;

namespace employee.api.Model
{
	public class EmployeeDbContext : DbContext
	{
		public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
		{
		}
		public DbSet<EmployeeModel> Employees { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Designation> Designations { get; set; }

	}
}
