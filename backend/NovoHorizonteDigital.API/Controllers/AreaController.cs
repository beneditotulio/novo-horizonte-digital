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
    public class AreaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AreaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<AreaDto>>> GetAreas()
        {
            var areas = await _context.Areas
                .Where(a => a.IsActive)
                .ToListAsync();

            var dtos = areas.Select(a => new AreaDto
            {
                Id = a.Id,
                Name = a.Name,
                Dimensions = a.Dimensions,
                AdhesionValue = a.AdhesionValue,
                MonthlyInstallment = a.MonthlyInstallment,
                PaymentPeriodMonths = a.PaymentPeriodMonths,
                HousingStandard = a.HousingStandard
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AreaDto>> GetArea(int id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null)
                return NotFound();

            var dto = new AreaDto
            {
                Id = area.Id,
                Name = area.Name,
                Dimensions = area.Dimensions,
                AdhesionValue = area.AdhesionValue,
                MonthlyInstallment = area.MonthlyInstallment,
                PaymentPeriodMonths = area.PaymentPeriodMonths,
                HousingStandard = area.HousingStandard
            };

            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AreaDto>> CreateArea([FromBody] CreateAreaRequest request)
        {
            var area = new Area
            {
                Name = request.Name,
                Dimensions = request.Dimensions,
                AdhesionValue = request.AdhesionValue,
                MonthlyInstallment = request.MonthlyInstallment,
                PaymentPeriodMonths = request.PaymentPeriodMonths,
                HousingStandard = request.HousingStandard,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Areas.Add(area);
            await _context.SaveChangesAsync();

            var dto = new AreaDto
            {
                Id = area.Id,
                Name = area.Name,
                Dimensions = area.Dimensions,
                AdhesionValue = area.AdhesionValue,
                MonthlyInstallment = area.MonthlyInstallment,
                PaymentPeriodMonths = area.PaymentPeriodMonths,
                HousingStandard = area.HousingStandard
            };

            return CreatedAtAction(nameof(GetArea), new { id = area.Id }, dto);
        }
    }
}
