using employee.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace employee.api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DepartmentMasterController : ControllerBase
	{
		private readonly EmployeeDbContext _context;

		public DepartmentMasterController(EmployeeDbContext context)
		{
			_context = context;
		}
		[HttpGet("GetAllDepartments")]
		public IActionResult GetDepartments()
		{
			var deptList = _context.Departments.ToList();
			return Ok(deptList);
		}

		[HttpPost("AddDepartment")]
		public IActionResult AddDepartment([FromBody] Department dept)
		{
			if (dept == null || string.IsNullOrEmpty(dept.departmentName))
			{
				return BadRequest("Invalid department data.");
			}

			// Check if department name already exists
			bool exists = _context.Departments
								  .Any(d => d.departmentName.ToLower() == dept.departmentName.ToLower());

			if (exists == false)
			{
				return BadRequest("Department name must be unique.");
			}

			_context.Departments.Add(dept);
			_context.SaveChanges();
			return Created("Department added successfully.",dept);
		}

		[HttpPut("UpdateDepartment/{id}")]
		public IActionResult UpdateDepartment(int id, [FromBody] Department dept)
		{
			var existingDept = _context.Departments.Find(id);
			if (existingDept == null)
			{
				return NotFound("Department not found.");
			}
			existingDept.departmentName = dept.departmentName;
			existingDept.isActive = dept.isActive;
			_context.SaveChanges();
			return Created("Department updated successfully.",dept);
		}

		[HttpDelete("DeleteDepartment/{id}")]
		public IActionResult DeleteDepartment(int id) {
			var existingDept = _context.Departments.Find(id);
			if (existingDept == null)
			{
				return NotFound("Department not found.");
			}
			_context.Departments.Remove(existingDept);
			_context.SaveChanges();
			return Created("Department deleted successfully.", existingDept);
		}
	}
}
