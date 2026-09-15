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
using System.Security.Claims;

namespace NovoHorizonteDigital.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("my-payments")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<List<MonthlyPaymentDto>>> GetMyPayments()
        {
            try
            {
                var clientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var payments = await _context.MonthlyPayments
                    .Include(mp => mp.Contract)
                    .Where(mp => mp.Contract.ClientId == clientId)
                    .OrderBy(mp => mp.MonthYear)
                    .ToListAsync();

                var dtos = payments.Select(p => new MonthlyPaymentDto
                {
                    Id = p.Id,
                    InstallmentNumber = p.InstallmentNumber,
                    TotalInstallments = p.TotalInstallments,
                    MonthYear = p.MonthYear,
                    ExpectedValue = p.ExpectedValue,
                    PaidValue = p.PaidValue,
                    Status = p.Status.ToString()
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("submit-proof")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult> SubmitPaymentProof([FromBody] SubmitPaymentRequest request)
        {
            try
            {
                var payment = await _context.MonthlyPayments.FindAsync(request.PaymentId);
                if (payment == null)
                    return NotFound("Payment not found");

                var clientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var contract = await _context.Contracts.FindAsync(payment.ContractId);

                if (contract.ClientId != clientId)
                    return Unauthorized("You can only submit proofs for your own payments");

                payment.ProofOfPaymentUrl = request.ProofOfPaymentUrl;
                payment.Status = PaymentStatus.UnderAnalysis;
                payment.SubmittedAt = DateTime.UtcNow;

                _context.MonthlyPayments.Update(payment);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Payment proof submitted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{paymentId}/validate")]
        [Authorize(Roles = "Operator")]
        public async Task<ActionResult> ValidatePayment(int paymentId, [FromBody] decimal paidAmount)
        {
            try
            {
                var operatorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                var payment = await _context.MonthlyPayments.FindAsync(paymentId);
                if (payment == null)
                    return NotFound("Payment not found");

                payment.Status = PaymentStatus.Paid;
                payment.PaidValue = paidAmount;
                payment.ValidatedAt = DateTime.UtcNow;
                payment.ValidatedByOperatorId = operatorId;

                _context.MonthlyPayments.Update(payment);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Payment validated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{contractId}/manual-payment")]
        [Authorize(Roles = "Operator")]
        public async Task<ActionResult> RegisterManualPayment(int contractId, [FromBody] decimal amount)
        {
            try
            {
                var operatorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var contract = await _context.Contracts
                    .Include(c => c.MonthlyPayments)
                    .FirstOrDefaultAsync(c => c.Id == contractId);

                if (contract == null)
                    return NotFound("Contract not found");

                var pendingPayment = contract.MonthlyPayments
                    .FirstOrDefault(p => p.Status == PaymentStatus.Pending);

                if (pendingPayment == null)
                    return BadRequest("No pending payment found");

                pendingPayment.Status = PaymentStatus.Paid;
                pendingPayment.PaidValue = amount;
                pendingPayment.ValidatedAt = DateTime.UtcNow;
                pendingPayment.ValidatedByOperatorId = operatorId;

                _context.MonthlyPayments.Update(pendingPayment);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Manual payment registered successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
