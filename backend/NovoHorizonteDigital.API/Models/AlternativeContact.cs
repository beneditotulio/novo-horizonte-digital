using System;

namespace NovoHorizonteDigital.API.Models
{
    public class AlternativeContact
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Relationship { get; set; } // Ex: Pai/Mãe, Cônjuge, Irmão/ã
        public int ContractId { get; set; }
        public Contract Contract { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
