using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovoHorizonteDigital.API.DTOs;
using NovoHorizonteDigital.API.Models;
using NovoHorizonteDigital.API.Services;

namespace NovoHorizonteDigital.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlotController : ControllerBase
    {
        private readonly IPlotService _plotService;

        public PlotController(IPlotService plotService)
        {
            _plotService = plotService;
        }

        [HttpGet("available/lot/{lotId}")]
        public async Task<ActionResult<List<PlotDto>>> GetAvailablePlotsByLot(int lotId)
        {
            try
            {
                var plots = await _plotService.GetAvailablePlotsByLotAsync(lotId);
                var dtos = plots.Select(p => new PlotDto
                {
                    Id = p.Id,
                    PlotNumber = p.PlotNumber,
                    AreaId = p.AreaId,
                    LotId = p.LotId,
                    Status = p.Status.ToString()
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("area/{areaId}/lot/{lotId}")]
        public async Task<ActionResult<List<PlotDto>>> GetPlotsByAreaAndLot(int areaId, int lotId)
        {
            try
            {
                var plots = await _plotService.GetPlotsByAreaAndLotAsync(areaId, lotId);
                var dtos = plots.Select(p => new PlotDto
                {
                    Id = p.Id,
                    PlotNumber = p.PlotNumber,
                    AreaId = p.AreaId,
                    LotId = p.LotId,
                    Status = p.Status.ToString()
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PlotDto>> CreatePlot([FromBody] CreatePlotRequest request)
        {
            try
            {
                var plot = await _plotService.CreatePlotAsync(request.AreaId, request.LotId, request.PlotNumber);
                
                var dto = new PlotDto
                {
                    Id = plot.Id,
                    PlotNumber = plot.PlotNumber,
                    AreaId = plot.AreaId,
                    LotId = plot.LotId,
                    Status = plot.Status.ToString()
                };

                return CreatedAtAction(nameof(GetAvailablePlotsByLot), new { lotId = plot.LotId }, dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("range")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<PlotDto>>> CreatePlotsInRange([FromBody] CreatePlotsInRangeRequest request)
        {
            try
            {
                var plots = await _plotService.CreatePlotsInRangeAsync(request.AreaId, request.LotId, 
                    request.StartNumber, request.EndNumber);

                var dtos = plots.Select(p => new PlotDto
                {
                    Id = p.Id,
                    PlotNumber = p.PlotNumber,
                    AreaId = p.AreaId,
                    LotId = p.LotId,
                    Status = p.Status.ToString()
                }).ToList();

                return Ok(new { 
                    message = $"{plots.Count} plots created successfully",
                    plots = dtos 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
