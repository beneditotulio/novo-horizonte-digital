using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NovoHorizonteDigital.API.Data;
using NovoHorizonteDigital.API.Models;

namespace NovoHorizonteDigital.API.Services
{
    public interface IContractService
    {
        Task<Contract> CreateContractAsync(int clientId, List<int> plotIds, string proofOfPaymentUrl, 
            string alternativeContactName, string alternativeContactPhone, string relationship);
        Task<Contract> GetContractByIdAsync(int contractId);
        Task<List<Contract>> GetContractsByStatusAsync(ContractStatus status);
        Task<Contract> ApproveContractAsync(int contractId, int operatorId);
        Task<bool> RejectContractAsync(int contractId);
        Task<decimal> CalculateTotalValueAsync(List<int> plotIds);
        Task<List<MonthlyPayment>> GenerateMonthlyPaymentsAsync(int contractId);
    }

    public class ContractService : IContractService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPlotService _plotService;

        public ContractService(ApplicationDbContext context, IPlotService plotService)
        {
            _context = context;
            _plotService = plotService;
        }

        public async Task<Contract> CreateContractAsync(int clientId, List<int> plotIds, string proofOfPaymentUrl,
            string alternativeContactName, string alternativeContactPhone, string relationship)
        {
            if (plotIds == null || plotIds.Count == 0)
                throw new ArgumentException("At least one plot must be selected");

            var client = await _context.Users.FindAsync(clientId);
            if (client == null || client.Role != UserRole.Client)
                throw new ArgumentException("Client not found");

            // Get plots and calculate total value
            var plots = await _context.Plots
                .Where(p => plotIds.Contains(p.Id))
                .Include(p => p.Area)
                .ToListAsync();

            if (plots.Count != plotIds.Count)
                throw new ArgumentException("One or more plots not found");

            // All plots must be from same area for now
            var areaId = plots[0].AreaId;
            if (plots.Any(p => p.AreaId != areaId))
                throw new ArgumentException("All plots must belong to the same area");

            var area = plots[0].Area;
            decimal totalAdhesionValue = area.AdhesionValue * plotIds.Count;
            decimal totalInstallmentValue = area.MonthlyInstallment * plotIds.Count * area.PaymentPeriodMonths;

            // Create contract
            var contract = new Contract
            {
                ContractCode = GenerateContractCode(),
                ClientId = clientId,
                TotalPlots = plotIds.Count,
                TotalAdhesionValue = totalAdhesionValue,
                TotalInstallmentValue = totalInstallmentValue,
                PaymentPeriodMonths = area.PaymentPeriodMonths,
                ProofOfPaymentUrl = proofOfPaymentUrl,
                Status = ContractStatus.PendingPayment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            // Add alternative contact
            var altContact = new AlternativeContact
            {
                ContractId = contract.Id,
                FullName = alternativeContactName,
                PhoneNumber = alternativeContactPhone,
                Relationship = relationship
            };
            _context.AlternativeContacts.Add(altContact);

            // Reserve plots
            await _plotService.ReservePlotsAsync(contract.Id, plotIds);

            // Add contract plots
            foreach (var plotId in plotIds)
            {
                var contractPlot = new ContractPlot
                {
                    ContractId = contract.Id,
                    PlotId = plotId,
                    AssignedAt = DateTime.UtcNow
                };
                _context.ContractPlots.Add(contractPlot);
            }

            await _context.SaveChangesAsync();

            // Generate monthly payments
            await GenerateMonthlyPaymentsAsync(contract.Id);

            return contract;
        }

        public async Task<Contract> GetContractByIdAsync(int contractId)
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ContractPlots)
                    .ThenInclude(cp => cp.Plot)
                        .ThenInclude(p => p.Area)
                .Include(c => c.AlternativeContact)
                .Include(c => c.MonthlyPayments)
                .FirstOrDefaultAsync(c => c.Id == contractId);
        }

        public async Task<List<Contract>> GetContractsByStatusAsync(ContractStatus status)
        {
            return await _context.Contracts
                .Where(c => c.Status == status)
                .Include(c => c.Client)
                .Include(c => c.ContractPlots)
                    .ThenInclude(cp => cp.Plot)
                .Include(c => c.MonthlyPayments)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Contract> ApproveContractAsync(int contractId, int operatorId)
        {
            var contract = await GetContractByIdAsync(contractId);
            if (contract == null)
                throw new ArgumentException("Contract not found");

            var @operator = await _context.Users.FindAsync(operatorId);
            if (@operator == null || @operator.Role != UserRole.Operator)
                throw new ArgumentException("Operator not found");

            var plotIds = contract.ContractPlots.Select(cp => cp.PlotId).ToList();
            await _plotService.AssignPlotsAsync(contractId, plotIds);

            contract.Status = ContractStatus.Active;
            contract.ApprovedAt = DateTime.UtcNow;
            contract.ApprovedByOperatorId = operatorId;
            contract.UpdatedAt = DateTime.UtcNow;

            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();

            return contract;
        }

        public async Task<bool> RejectContractAsync(int contractId)
        {
            var contract = await GetContractByIdAsync(contractId);
            if (contract == null)
                throw new ArgumentException("Contract not found");

            await _plotService.ReleasePlotsAsync(contractId);

            contract.Status = ContractStatus.Cancelled;
            contract.UpdatedAt = DateTime.UtcNow;

            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> CalculateTotalValueAsync(List<int> plotIds)
        {
            var plots = await _context.Plots
                .Include(p => p.Area)
                .Where(p => plotIds.Contains(p.Id))
                .ToListAsync();

            if (plots.Count == 0)
                return 0;

            return plots[0].Area.AdhesionValue * plotIds.Count;
        }

        public async Task<List<MonthlyPayment>> GenerateMonthlyPaymentsAsync(int contractId)
        {
            var contract = await _context.Contracts.FindAsync(contractId);
            if (contract == null)
                throw new ArgumentException("Contract not found");

            var payments = new List<MonthlyPayment>();
            var monthlyValue = contract.TotalInstallmentValue / contract.PaymentPeriodMonths;

            for (int i = 1; i <= contract.PaymentPeriodMonths; i++)
            {
                var payment = new MonthlyPayment
                {
                    ContractId = contractId,
                    InstallmentNumber = i,
                    TotalInstallments = contract.PaymentPeriodMonths,
                    MonthYear = DateTime.UtcNow.AddMonths(i - 1),
                    ExpectedValue = monthlyValue,
                    Status = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };
                payments.Add(payment);
            }

            _context.MonthlyPayments.AddRange(payments);
            await _context.SaveChangesAsync();

            return payments;
        }

        private string GenerateContractCode()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var random = new Random().Next(1000, 9999);
            return $"CT-{DateTime.UtcNow:yyyyMMdd}-{random}";
        }
    }
}
