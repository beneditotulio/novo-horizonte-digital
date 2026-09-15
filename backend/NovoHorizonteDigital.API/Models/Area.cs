using System;
using System.Collections.Generic;

namespace NovoHorizonteDigital.API.Models
{
    public class Area
    {
        public int Id { get; set; }
        public string Name { get; set; } // Ex: Terreno Jovem, Área-2 Executivo
        public string Dimensions { get; set; } // Ex: 15x30, 20x40
        public decimal AdhesionValue { get; set; } // Valor de adesão
        public decimal MonthlyInstallment { get; set; } // Prestação mensal
        public int PaymentPeriodMonths { get; set; } // 12, 15, 24 meses
        public string HousingStandard { get; set; } // Alto, Médio, Básico
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<Lot> Lots { get; set; } = new List<Lot>();
        public ICollection<Plot> Plots { get; set; } = new List<Plot>();
    }
}
