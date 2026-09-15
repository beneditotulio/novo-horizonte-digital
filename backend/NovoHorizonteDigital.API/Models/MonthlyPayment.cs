using System;

namespace NovoHorizonteDigital.API.Models
{
    public class MonthlyPayment
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public Contract Contract { get; set; }
        public int InstallmentNumber { get; set; } // Ex: 1 de 24
        public int TotalInstallments { get; set; } // Ex: 24
        public DateTime MonthYear { get; set; }
        public decimal ExpectedValue { get; set; }
        public decimal PaidValue { get; set; } = 0;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string ProofOfPaymentUrl { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ValidatedAt { get; set; }
        public int? ValidatedByOperatorId { get; set; }
        public User? ValidatedByOperator { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum PaymentStatus
    {
        Pending,
        UnderAnalysis,
        Paid,
        Late
    }
}
