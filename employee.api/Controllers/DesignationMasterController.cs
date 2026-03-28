using employee.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace employee.api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DesignationMasterController : ControllerBase
	{
		private readonly EmployeeDbContext _context;

		public DesignationMasterController(EmployeeDbContext context)
		{
			_context = context;
		}
		// ===============================
		// GET ALL
		// ===============================
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var list = await _context.Designations.ToListAsync();
				return Ok(list);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error: {ex.Message}");
			}
		}

		// ===============================
		// GET BY ID
		// ===============================
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var data = await _context.Designations.FindAsync(id);

				if (data == null)
					return NotFound("Designation not found.");

				return Ok(data);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error: {ex.Message}");
			}
		}

		// ===============================
		// CREATE
		// ===============================
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Designation model)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				await _context.Designations.AddAsync(model);
				await _context.SaveChangesAsync();

				return CreatedAtAction(nameof(GetById), new { id = model.designationId }, model);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error: {ex.Message}");
			}
		}


		// ===============================
		// UPDATE
		// ===============================
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] Designation model)
		{
			try
			{
				if (id != model.designationId)
					return BadRequest("ID mismatch.");

				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var existing = await _context.Designations.FindAsync(id);
				if (existing == null)
					return NotFound("Designation not found.");

				existing.departmentId = model.departmentId;
				existing.designationName = model.designationName;

				await _context.SaveChangesAsync();

				return Ok("Designation updated successfully.");
			}
			catch (DbUpdateException ex)
			{
				return StatusCode(500, $"Database error: {ex.Message}");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error: {ex.Message}");
			}
		}


		// ===============================
		// DELETE
		// ===============================
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var data = await _context.Designations.FindAsync(id);
				if (data == null)
					return NotFound("Designation not found.");

				_context.Designations.Remove(data);
				await _context.SaveChangesAsync();

				return Ok("Designation deleted successfully.");
			}
			catch (DbUpdateException ex)
			{
				return StatusCode(500, $"Database constraint error: {ex.Message}");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error: {ex.Message}");
			}
		}



		// ===============================
		// FILTER API
		// Example:
		// api/Designation/filter?departmentId=1&name=manager
		// ===============================
		[HttpGet("filter")]
		public async Task<IActionResult> Filter(int? departmentId, string? name)
		{
			try
			{
				var query = _context.Designations.AsQueryable();

				if (departmentId.HasValue)
					query = query.Where(x => x.departmentId == departmentId.Value);

				if (!string.IsNullOrEmpty(name))
					query = query.Where(x => x.designationName.Contains(name));

				var result = await query.ToListAsync();

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error: {ex.Message}");
			}
		}






	}
}
