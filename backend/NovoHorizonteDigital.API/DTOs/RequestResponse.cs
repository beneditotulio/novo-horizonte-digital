namespace NovoHorizonteDigital.API.DTOs
{
    // Auth DTOs
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }

    // Area DTOs
    public class CreateAreaRequest
    {
        public string Name { get; set; }
        public string Dimensions { get; set; }
        public decimal AdhesionValue { get; set; }
        public decimal MonthlyInstallment { get; set; }
        public int PaymentPeriodMonths { get; set; }
        public string HousingStandard { get; set; }
    }

    public class AreaDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Dimensions { get; set; }
        public decimal AdhesionValue { get; set; }
        public decimal MonthlyInstallment { get; set; }
        public int PaymentPeriodMonths { get; set; }
        public string HousingStandard { get; set; }
    }

    // Lot DTOs
    public class CreateLotRequest
    {
        public string Name { get; set; }
        public int AreaId { get; set; }
    }

    public class LotDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AreaId { get; set; }
    }

    // Plot DTOs
    public class CreatePlotRequest
    {
        public int AreaId { get; set; }
        public int LotId { get; set; }
        public string PlotNumber { get; set; }
    }

    public class CreatePlotsInRangeRequest
    {
        public int AreaId { get; set; }
        public int LotId { get; set; }
        public int StartNumber { get; set; }
        public int EndNumber { get; set; }
    }

    public class PlotDto
    {
        public int Id { get; set; }
        public string PlotNumber { get; set; }
        public int AreaId { get; set; }
        public int LotId { get; set; }
        public string Status { get; set; }
    }

    // Contract DTOs
    public class CreateContractRequest
    {
        public List<int> PlotIds { get; set; }
        public string ProofOfPaymentUrl { get; set; }
        public string AlternativeContactName { get; set; }
        public string AlternativeContactPhone { get; set; }
        public string Relationship { get; set; }
    }

    public class ContractDto
    {
        public int Id { get; set; }
        public string ContractCode { get; set; }
        public string ClientName { get; set; }
        public int TotalPlots { get; set; }
        public decimal TotalAdhesionValue { get; set; }
        public decimal TotalInstallmentValue { get; set; }
        public int PaymentPeriodMonths { get; set; }
        public string Status { get; set; }
        public List<string> PlotNumbers { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ContractDetailDto
    {
        public int Id { get; set; }
        public string ContractCode { get; set; }
        public UserDto Client { get; set; }
        public int TotalPlots { get; set; }
        public decimal TotalAdhesionValue { get; set; }
        public decimal TotalInstallmentValue { get; set; }
        public int PaymentPeriodMonths { get; set; }
        public string Status { get; set; }
        public List<PlotDto> Plots { get; set; }
        public AlternativeContactDto AlternativeContact { get; set; }
        public List<MonthlyPaymentDto> MonthlyPayments { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AlternativeContactDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Relationship { get; set; }
    }

    // Monthly Payment DTOs
    public class MonthlyPaymentDto
    {
        public int Id { get; set; }
        public int InstallmentNumber { get; set; }
        public int TotalInstallments { get; set; }
        public DateTime MonthYear { get; set; }
        public decimal ExpectedValue { get; set; }
        public decimal PaidValue { get; set; }
        public string Status { get; set; }
    }

    public class SubmitPaymentRequest
    {
        public int PaymentId { get; set; }
        public string ProofOfPaymentUrl { get; set; }
    }
}
