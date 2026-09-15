using System;

namespace NovoHorizonteDigital.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public UserRole Role { get; set; } // Admin, Operator, Client
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Client-specific fields
        public string IdentificationNumber { get; set; } // BI
        public string TaxNumber { get; set; } // NUIT
        public string Address { get; set; }
        public string IdentificationDocumentUrl { get; set; }
        public string TaxDocumentUrl { get; set; }
    }

    public enum UserRole
    {
        Admin,
        Operator,
        Client
    }
}
