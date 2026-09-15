using System;
using System.Collections.Generic;

namespace NovoHorizonteDigital.API.Models
{
    public class Lot
    {
        public int Id { get; set; }
        public string Name { get; set; } // Ex: Lote 01, Lote 02
        public int AreaId { get; set; }
        public Area Area { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<Plot> Plots { get; set; } = new List<Plot>();
    }
}
