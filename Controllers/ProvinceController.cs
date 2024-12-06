using App.Models;
using App.Models.Provinces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace App.Controllers
{
    [Route("api/provinces")]
    [ApiController]
    public class ProvinceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProvinceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _context.Provinces
                .Select(p => new { p.Code, p.FullName })
                .OrderBy(p => p.FullName)
                .ToListAsync();
            return Ok(provinces);
        }

        [HttpGet("{provinceCode}/districts")]
        public async Task<IActionResult> GetDistricts(string provinceCode)
        {
            var districts = await _context.Districts
                .Where(d => d.ProvinceCode == provinceCode)
                .Select(d => new { d.Code, d.FullName })
                .OrderBy(d => d.FullName)
                .ToListAsync();
            return Ok(districts);
        }

        [HttpGet("districts/{districtCode}/wards")]
        public async Task<IActionResult> GetWards(string districtCode)
        {
            var wards = await _context.Wards
                .Where(w => w.DistrictCode == districtCode)
                .Select(w => new { w.Code, w.FullName })
                .OrderBy(w => w.FullName)
                .ToListAsync();
            return Ok(wards);
        }
    }

}
