using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NovoHorizonteDigital.API.Data;
using NovoHorizonteDigital.API.Models;

namespace NovoHorizonteDigital.API.Services
{
    public interface IPlotService
    {
        Task<Plot> CreatePlotAsync(int areaId, int lotId, string plotNumber);
        Task<List<Plot>> CreatePlotsInRangeAsync(int areaId, int lotId, int startNumber, int endNumber);
        Task<List<Plot>> GetAvailablePlotsByLotAsync(int lotId);
        Task<List<Plot>> GetPlotsByAreaAndLotAsync(int areaId, int lotId);
        Task<Plot> GetPlotByIdAsync(int plotId);
        Task<bool> ReservePlotsAsync(int contractId, List<int> plotIds);
        Task<bool> ReleasePlotsAsync(int contractId);
        Task<bool> AssignPlotsAsync(int contractId, List<int> plotIds);
    }

    public class PlotService : IPlotService
    {
        private readonly ApplicationDbContext _context;

        public PlotService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Plot> CreatePlotAsync(int areaId, int lotId, string plotNumber)
        {
            // Verify Area and Lot exist
            var area = await _context.Areas.FindAsync(areaId);
            if (area == null)
                throw new ArgumentException($"Area with ID {areaId} not found");

            var lot = await _context.Lots.FindAsync(lotId);
            if (lot == null || lot.AreaId != areaId)
                throw new ArgumentException($"Lot with ID {lotId} not found in Area {areaId}");

            // Check if plot number already exists in this lot
            var existingPlot = await _context.Plots
                .FirstOrDefaultAsync(p => p.PlotNumber == plotNumber && p.LotId == lotId);
            
            if (existingPlot != null)
                throw new InvalidOperationException($"Plot number {plotNumber} already exists in Lot {lotId}");

            var plot = new Plot
            {
                PlotNumber = plotNumber,
                AreaId = areaId,
                LotId = lotId,
                Status = PlotStatus.Available
            };

            _context.Plots.Add(plot);
            await _context.SaveChangesAsync();
            return plot;
        }

        public async Task<List<Plot>> CreatePlotsInRangeAsync(int areaId, int lotId, int startNumber, int endNumber)
        {
            if (startNumber > endNumber)
                throw new ArgumentException("Start number must be less than or equal to end number");

            var area = await _context.Areas.FindAsync(areaId);
            if (area == null)
                throw new ArgumentException($"Area with ID {areaId} not found");

            var lot = await _context.Lots.FindAsync(lotId);
            if (lot == null || lot.AreaId != areaId)
                throw new ArgumentException($"Lot with ID {lotId} not found in Area {areaId}");

            var plots = new List<Plot>();
            var existingPlotNumbers = await _context.Plots
                .Where(p => p.LotId == lotId)
                .Select(p => p.PlotNumber)
                .ToListAsync();

            for (int i = startNumber; i <= endNumber; i++)
            {
                string plotNumber = i.ToString();
                
                if (!existingPlotNumbers.Contains(plotNumber))
                {
                    var plot = new Plot
                    {
                        PlotNumber = plotNumber,
                        AreaId = areaId,
                        LotId = lotId,
                        Status = PlotStatus.Available
                    };
                    plots.Add(plot);
                }
            }

            if (plots.Count > 0)
            {
                _context.Plots.AddRange(plots);
                await _context.SaveChangesAsync();
            }

            return plots;
        }

        public async Task<List<Plot>> GetAvailablePlotsByLotAsync(int lotId)
        {
            return await _context.Plots
                .Where(p => p.LotId == lotId && p.Status == PlotStatus.Available)
                .OrderBy(p => int.Parse(p.PlotNumber))
                .ToListAsync();
        }

        public async Task<List<Plot>> GetPlotsByAreaAndLotAsync(int areaId, int lotId)
        {
            return await _context.Plots
                .Where(p => p.AreaId == areaId && p.LotId == lotId)
                .OrderBy(p => int.Parse(p.PlotNumber))
                .ToListAsync();
        }

        public async Task<Plot> GetPlotByIdAsync(int plotId)
        {
            return await _context.Plots.FindAsync(plotId);
        }

        public async Task<bool> ReservePlotsAsync(int contractId, List<int> plotIds)
        {
            var plots = await _context.Plots
                .Where(p => plotIds.Contains(p.Id))
                .ToListAsync();

            // Check if all plots are available
            if (plots.Any(p => p.Status != PlotStatus.Available))
                throw new InvalidOperationException("One or more plots are not available for reservation");

            // Reserve all plots
            foreach (var plot in plots)
            {
                plot.Status = PlotStatus.Reserved;
                plot.ReservedByContractId = contractId;
                plot.ReservedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReleasePlotsAsync(int contractId)
        {
            var plots = await _context.Plots
                .Where(p => p.ReservedByContractId == contractId)
                .ToListAsync();

            foreach (var plot in plots)
            {
                plot.Status = PlotStatus.Available;
                plot.ReservedByContractId = null;
                plot.ReservedAt = null;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignPlotsAsync(int contractId, List<int> plotIds)
        {
            var plots = await _context.Plots
                .Where(p => plotIds.Contains(p.Id))
                .ToListAsync();

            // Change status to Occupied
            foreach (var plot in plots)
            {
                plot.Status = PlotStatus.Occupied;
                plot.ReservedByContractId = contractId;
                plot.ReservedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
