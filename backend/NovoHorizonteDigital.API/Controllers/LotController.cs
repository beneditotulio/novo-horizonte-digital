using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovoHorizonteDigital.API.Data;
using NovoHorizonteDigital.API.DTOs;
using NovoHorizonteDigital.API.Models;

namespace NovoHorizonteDigital.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LotController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LotController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("area/{areaId}")]
        public async Task<ActionResult<List<LotDto>>> GetLotsByArea(int areaId)
        {
            var lots = await _context.Lots
                .Where(l => l.AreaId == areaId && l.IsActive)
                .ToListAsync();

            var dtos = lots.Select(l => new LotDto
            {
                Id = l.Id,
                Name = l.Name,
                AreaId = l.AreaId
            }).ToList();

            return Ok(dtos);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<LotDto>> CreateLot([FromBody] CreateLotRequest request)
        {
            var area = await _context.Areas.FindAsync(request.AreaId);
            if (area == null)
                return NotFound("Area not found");

            var lot = new Lot
            {
                Name = request.Name,
                AreaId = request.AreaId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Lots.Add(lot);
            await _context.SaveChangesAsync();

            var dto = new LotDto
            {
                Id = lot.Id,
                Name = lot.Name,
                AreaId = lot.AreaId
            };

            return CreatedAtAction(nameof(GetLotsByArea), new { areaId = lot.AreaId }, dto);
        }
    }
}
