using System;

namespace NovoHorizonteDigital.API.Models
{
    public class Plot
    {
        public int Id { get; set; }
        public string PlotNumber { get; set; } // Ex: 600, 601, 602
        public int AreaId { get; set; }
        public Area Area { get; set; }
        public int LotId { get; set; }
        public Lot Lot { get; set; }
        public PlotStatus Status { get; set; } = PlotStatus.Available; // Available, Reserved, Occupied
        public int? ReservedByContractId { get; set; } // Temporary reservation
        public DateTime? ReservedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Contract? Contract { get; set; }
    }

    public enum PlotStatus
    {
        Available,
        Reserved,
        Occupied
    }
}
