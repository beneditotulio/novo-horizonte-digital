using NovoHorizonteDigital.API.Models;

namespace NovoHorizonteDigital.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Create database if not exists
            context.Database.EnsureCreated();

            // Check if data already exists
            if (context.Users.Any())
            {
                return; // Database has been seeded
            }

            // Create default admin user
            var adminPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123");
            var admin = new User
            {
                Email = "admin@novo-horizonte.com",
                PasswordHash = adminPassword,
                FullName = "Administrador Sistema",
                PhoneNumber = "+258 21 306 000",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Create default operator user
            var operatorPassword = BCrypt.Net.BCrypt.HashPassword("Operator@123");
            var operatorUser = new User
            {
                Email = "operador@novo-horizonte.com",
                PasswordHash = operatorPassword,
                FullName = "Operador Comercial",
                PhoneNumber = "+258 21 306 001",
                Role = UserRole.Operator,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(admin);
            context.Users.Add(operatorUser);

            // Create sample areas
            var area1 = new Area
            {
                Name = "Terreno Jovem",
                Dimensions = "15x30",
                AdhesionValue = 50000,
                MonthlyInstallment = 2500,
                PaymentPeriodMonths = 24,
                HousingStandard = "Básico",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var area2 = new Area
            {
                Name = "Área-2 Executivo",
                Dimensions = "20x30",
                AdhesionValue = 100000,
                MonthlyInstallment = 5000,
                PaymentPeriodMonths = 24,
                HousingStandard = "Médio",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var area3 = new Area
            {
                Name = "Área-1 Premium",
                Dimensions = "20x40",
                AdhesionValue = 150000,
                MonthlyInstallment = 7500,
                PaymentPeriodMonths = 24,
                HousingStandard = "Alto",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Areas.AddRange(area1, area2, area3);
            context.SaveChanges();

            // Create sample lots
            var lot1Area1 = new Lot
            {
                Name = "Lote 01",
                AreaId = area1.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var lot2Area2 = new Lot
            {
                Name = "Lote 01",
                AreaId = area2.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var lot3Area3 = new Lot
            {
                Name = "Lote 01",
                AreaId = area3.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Lots.AddRange(lot1Area1, lot2Area2, lot3Area3);
            context.SaveChanges();

            // Create sample plots for each lot (range from 100-120)
            var plots1 = new List<Plot>();
            for (int i = 100; i <= 120; i++)
            {
                plots1.Add(new Plot
                {
                    PlotNumber = i.ToString(),
                    AreaId = area1.Id,
                    LotId = lot1Area1.Id,
                    Status = PlotStatus.Available,
                    CreatedAt = DateTime.UtcNow
                });
            }

            var plots2 = new List<Plot>();
            for (int i = 200; i <= 230; i++)
            {
                plots2.Add(new Plot
                {
                    PlotNumber = i.ToString(),
                    AreaId = area2.Id,
                    LotId = lot2Area2.Id,
                    Status = PlotStatus.Available,
                    CreatedAt = DateTime.UtcNow
                });
            }

            var plots3 = new List<Plot>();
            for (int i = 300; i <= 335; i++)
            {
                plots3.Add(new Plot
                {
                    PlotNumber = i.ToString(),
                    AreaId = area3.Id,
                    LotId = lot3Area3.Id,
                    Status = PlotStatus.Available,
                    CreatedAt = DateTime.UtcNow
                });
            }

            context.Plots.AddRange(plots1);
            context.Plots.AddRange(plots2);
            context.Plots.AddRange(plots3);
            context.SaveChanges();
        }
    }
}
