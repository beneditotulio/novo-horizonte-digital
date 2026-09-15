using System;
using System.Collections.Generic;

namespace NovoHorizonteDigital.API.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public string ContractCode { get; set; } // Unique identifier
        public int ClientId { get; set; }
        public User Client { get; set; }
        public int TotalPlots { get; set; } // Total terrenos selecionados
        public decimal TotalAdhesionValue { get; set; }
        public decimal TotalInstallmentValue { get; set; } // Valor total das mensalidades
        public int PaymentPeriodMonths { get; set; }
        public ContractStatus Status { get; set; } = ContractStatus.PendingPayment;
        public string ProofOfPaymentUrl { get; set; }
        public string ContractPdfUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
        public int? ApprovedByOperatorId { get; set; }
        public User? ApprovedByOperator { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<ContractPlot> ContractPlots { get; set; } = new List<ContractPlot>();
        public ICollection<Monthly Payment> MonthlyPayments { get; set; } = new List<MonthlyPayment>();
        public AlternativeContact AlternativeContact { get; set; }
    }

    public enum ContractStatus
    {
        PendingPayment,
        UnderAnalysis,
        Active,
        Cancelled
    }
}
