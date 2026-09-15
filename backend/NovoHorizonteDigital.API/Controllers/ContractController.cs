using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovoHorizonteDigital.API.DTOs;
using NovoHorizonteDigital.API.Models;
using NovoHorizonteDigital.API.Services;
using System.Security.Claims;

namespace NovoHorizonteDigital.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly IPdfService _pdfService;

        public ContractController(IContractService contractService, IPdfService pdfService)
        {
            _contractService = contractService;
            _pdfService = pdfService;
        }

        [HttpPost("reserve")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<ContractDto>> ReserveTerrains([FromBody] CreateContractRequest request)
        {
            try
            {
                var clientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                var contract = await _contractService.CreateContractAsync(
                    clientId,
                    request.PlotIds,
                    request.ProofOfPaymentUrl,
                    request.AlternativeContactName,
                    request.AlternativeContactPhone,
                    request.Relationship
                );

                // Generate PDF
                var pdfBytes = await _pdfService.GenerateContractPdfAsync(contract.Id);
                var contractPdfPath = $"/contracts/{contract.ContractCode}_preview.pdf";

                return Ok(new
                {
                    message = "Reservation created successfully",
                    contractId = contract.Id,
                    contractCode = contract.ContractCode,
                    status = contract.Status.ToString(),
                    totalPlots = contract.TotalPlots,
                    totalAdhesionValue = contract.TotalAdhesionValue,
                    totalInstallmentValue = contract.TotalInstallmentValue
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Operator")]
        public async Task<ActionResult<List<ContractDto>>> GetPendingContracts()
        {
            try
            {
                var contracts = await _contractService.GetContractsByStatusAsync(ContractStatus.PendingPayment);
                
                var dtos = contracts.Select(c => new ContractDto
                {
                    Id = c.Id,
                    ContractCode = c.ContractCode,
                    ClientName = c.Client.FullName,
                    TotalPlots = c.TotalPlots,
                    TotalAdhesionValue = c.TotalAdhesionValue,
                    TotalInstallmentValue = c.TotalInstallmentValue,
                    PaymentPeriodMonths = c.PaymentPeriodMonths,
                    Status = c.Status.ToString(),
                    PlotNumbers = c.ContractPlots.Select(cp => cp.Plot.PlotNumber).ToList(),
                    CreatedAt = c.CreatedAt
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{contractId}")]
        public async Task<ActionResult<ContractDetailDto>> GetContractDetails(int contractId)
        {
            try
            {
                var contract = await _contractService.GetContractByIdAsync(contractId);
                if (contract == null)
                    return NotFound();

                var dto = new ContractDetailDto
                {
                    Id = contract.Id,
                    ContractCode = contract.ContractCode,
                    Client = new UserDto
                    {
                        Id = contract.Client.Id,
                        FullName = contract.Client.FullName,
                        Email = contract.Client.Email,
                        PhoneNumber = contract.Client.PhoneNumber,
                        Role = contract.Client.Role.ToString()
                    },
                    TotalPlots = contract.TotalPlots,
                    TotalAdhesionValue = contract.TotalAdhesionValue,
                    TotalInstallmentValue = contract.TotalInstallmentValue,
                    PaymentPeriodMonths = contract.PaymentPeriodMonths,
                    Status = contract.Status.ToString(),
                    Plots = contract.ContractPlots.Select(cp => new PlotDto
                    {
                        Id = cp.Plot.Id,
                        PlotNumber = cp.Plot.PlotNumber,
                        AreaId = cp.Plot.AreaId,
                        LotId = cp.Plot.LotId,
                        Status = cp.Plot.Status.ToString()
                    }).ToList(),
                    AlternativeContact = contract.AlternativeContact != null ? new AlternativeContactDto
                    {
                        Id = contract.AlternativeContact.Id,
                        FullName = contract.AlternativeContact.FullName,
                        PhoneNumber = contract.AlternativeContact.PhoneNumber,
                        Relationship = contract.AlternativeContact.Relationship
                    } : null,
                    MonthlyPayments = contract.MonthlyPayments.Select(mp => new MonthlyPaymentDto
                    {
                        Id = mp.Id,
                        InstallmentNumber = mp.InstallmentNumber,
                        TotalInstallments = mp.TotalInstallments,
                        MonthYear = mp.MonthYear,
                        ExpectedValue = mp.ExpectedValue,
                        PaidValue = mp.PaidValue,
                        Status = mp.Status.ToString()
                    }).ToList(),
                    CreatedAt = contract.CreatedAt
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{contractId}/approve")]
        [Authorize(Roles = "Operator")]
        public async Task<ActionResult> ApproveContract(int contractId)
        {
            try
            {
                var operatorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var contract = await _contractService.ApproveContractAsync(contractId, operatorId);

                // Generate PDF
                var pdfBytes = await _pdfService.GenerateContractPdfAsync(contract.Id);

                return Ok(new
                {
                    message = "Contract approved successfully",
                    contractCode = contract.ContractCode,
                    status = contract.Status.ToString()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{contractId}/reject")]
        [Authorize(Roles = "Operator")]
        public async Task<ActionResult> RejectContract(int contractId)
        {
            try
            {
                await _contractService.RejectContractAsync(contractId);
                return Ok(new { message = "Contract rejected successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{contractId}/pdf")]
        public async Task<ActionResult> GetContractPdf(int contractId)
        {
            try
            {
                var pdfBytes = await _pdfService.GenerateContractPdfAsync(contractId);
                return File(pdfBytes, "application/pdf", $"Contract_{contractId}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
