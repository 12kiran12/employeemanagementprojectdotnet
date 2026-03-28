using employee.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace employee.api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmployeeMasterController : ControllerBase
	{
		private readonly EmployeeDbContext _context;

		public EmployeeMasterController(EmployeeDbContext context)
		{
			_context = context;
		}

		// =====================================
		// GET ALL (Normal Get)
		// =====================================
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var employees = await _context.Employees.ToListAsync();
				return Ok(employees);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}


		// =====================================
		// GET BY ID
		// =====================================
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var employee = await _context.Employees.FindAsync(id);

				if (employee == null)
					return NotFound("Employee not found.");

				return Ok(employee);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}


		// =====================================
		// CREATE
		// =====================================
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] EmployeeModel model)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				// Unique ContactNo check
				if (await _context.Employees.AnyAsync(x => x.contactNo == model.contactNo))
					return BadRequest("Contact number already exists.");

				// Unique Email check
				if (!string.IsNullOrEmpty(model.email) &&
					await _context.Employees.AnyAsync(x => x.email == model.email))
					return BadRequest("Email already exists.");

				model.createdDate = DateTime.Now;

				await _context.Employees.AddAsync(model);
				await _context.SaveChangesAsync();

				return CreatedAtAction(nameof(GetById), new { id = model.employeeId }, model);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}



		// =====================================
		// UPDATE
		// =====================================
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] EmployeeModel model)
		{
			try
			{
				if (id != model.employeeId)
					return BadRequest("ID mismatch.");

				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var existing = await _context.Employees.FindAsync(id);
				if (existing == null)
					return NotFound("Employee not found.");

				// Unique ContactNo check
				if (await _context.Employees
					.AnyAsync(x => x.contactNo == model.contactNo && x.employeeId != id))
					return BadRequest("Contact number already exists.");

				// Unique Email check
				if (!string.IsNullOrEmpty(model.email) &&
					await _context.Employees
					.AnyAsync(x => x.email == model.email && x.employeeId != id))
					return BadRequest("Email already exists.");

				existing.name = model.name;
				existing.contactNo = model.contactNo;
				existing.email = model.email;
				existing.city = model.city;
				existing.state = model.state;
				existing.pincode = model.pincode;
				existing.altContactNo = model.altContactNo;
				existing.address = model.address;
				existing.designationId = model.designationId;
				existing.modifiedDate = DateTime.Now;

				await _context.SaveChangesAsync();

				return Ok("Employee updated successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}



		// =====================================
		// DELETE
		// =====================================
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var employee = await _context.Employees.FindAsync(id);
				if (employee == null)
					return NotFound("Employee not found.");

				_context.Employees.Remove(employee);
				await _context.SaveChangesAsync();

				return Ok("Employee deleted successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}


		// =====================================
		// FILTER + SORT + PAGINATION API
		// Example:
		// api/Employee/search?name=kiran&pageNumber=1&pageSize=5&sortBy=name&sortOrder=asc
		// =====================================
		[HttpGet("search")]
		public async Task<IActionResult> Search(
			string? name,
			string? city,
			string? sortBy = "employeeId",
			string? sortOrder = "asc",
			int pageNumber = 1,
			int pageSize = 10)
		{
			try
			{
				var query = _context.Employees.AsQueryable();

				// Filtering
				if (!string.IsNullOrEmpty(name))
					query = query.Where(x => x.name.Contains(name));

				if (!string.IsNullOrEmpty(city))
					query = query.Where(x => x.city.Contains(city));

				// Sorting
				switch (sortBy.ToLower())
				{
					case "name":
						query = sortOrder.ToLower() == "desc"
							? query.OrderByDescending(x => x.name)
							: query.OrderBy(x => x.name);
						break;

					case "createddate":
						query = sortOrder.ToLower() == "desc"
							? query.OrderByDescending(x => x.createdDate)
							: query.OrderBy(x => x.createdDate);
						break;

					default:
						query = query.OrderBy(x => x.employeeId);
						break;
				}

				// Pagination
				var totalRecords = await query.CountAsync();

				var data = await query
					.Skip((pageNumber - 1) * pageSize)
					.Take(pageSize)
					.ToListAsync();

				return Ok(new
				{
					totalRecords,
					pageNumber,
					pageSize,
					data
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}


		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto model)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var user = await _context.Employees
					.FirstOrDefaultAsync(x => x.email == model.email && x.contactNo == model.contactNo);

				if (user == null)
					return Unauthorized("Invalid credential.");

				return Ok(new
				{
					message = "Login successful",
					data = new
					{
						user.employeeId,
						user.name,
						user.email,
						user.contactNo,
						user.designationId,
						user.designationName,
						user.role
					}
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}


	}
}
