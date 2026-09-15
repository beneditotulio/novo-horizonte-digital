using System;

namespace NovoHorizonteDigital.API.Models
{
    public class ContractPlot
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public Contract Contract { get; set; }
        public int PlotId { get; set; }
        public Plot Plot { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
