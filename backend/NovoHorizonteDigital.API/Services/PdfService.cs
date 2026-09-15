using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.EntityFrameworkCore;
using NovoHorizonteDigital.API.Data;
using NovoHorizonteDigital.API.Models;

namespace NovoHorizonteDigital.API.Services
{
    public interface IPdfService
    {
        Task<byte[]> GenerateContractPdfAsync(int contractId);
        Task<string> SaveContractPdfAsync(int contractId, string savePath);
    }

    public class PdfService : IPdfService
    {
        private readonly ApplicationDbContext _context;

        public PdfService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> GenerateContractPdfAsync(int contractId)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ContractPlots)
                    .ThenInclude(cp => cp.Plot)
                        .ThenInclude(p => p.Area)
                .Include(c => c.ContractPlots)
                    .ThenInclude(cp => cp.Plot)
                        .ThenInclude(p => p.Lot)
                .Include(c => c.AlternativeContact)
                .FirstOrDefaultAsync(c => c.Id == contractId);

            if (contract == null)
                throw new ArgumentException("Contract not found");

            using (var memoryStream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(memoryStream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Set margins
                document.SetMargins(20, 20, 20, 20);

                // Add title
                var title = new Paragraph("CONTRATO DE ADESÃO E AQUISIÇÃO DE TERRENO")
                    .SetFontSize(14)
                    .SetBold()
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                document.Add(title);

                document.Add(new Paragraph("\n"));

                // Contract info
                document.Add(new Paragraph($"Código do Contrato: {contract.ContractCode}").SetFontSize(11));
                document.Add(new Paragraph($"Data de Emissão: {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}").SetFontSize(11));
                document.Add(new Paragraph("\n"));

                // Client info
                document.Add(new Paragraph("DADOS DO CLIENTE").SetBold().SetFontSize(12));
                document.Add(new Paragraph($"Nome Completo: {contract.Client.FullName}").SetFontSize(10));
                document.Add(new Paragraph($"Número de Identificação (BI): {contract.Client.IdentificationNumber}").SetFontSize(10));
                document.Add(new Paragraph($"NUIT: {contract.Client.TaxNumber}").SetFontSize(10));
                document.Add(new Paragraph($"Email: {contract.Client.Email}").SetFontSize(10));
                document.Add(new Paragraph($"Telefone: {contract.Client.PhoneNumber}").SetFontSize(10));
                document.Add(new Paragraph($"Morada: {contract.Client.Address}").SetFontSize(10));
                document.Add(new Paragraph("\n"));

                // Alternative contact
                if (contract.AlternativeContact != null)
                {
                    document.Add(new Paragraph("CONTACTO ALTERNATIVO (FAMILIAR)").SetBold().SetFontSize(12));
                    document.Add(new Paragraph($"Nome: {contract.AlternativeContact.FullName}").SetFontSize(10));
                    document.Add(new Paragraph($"Telefone: {contract.AlternativeContact.PhoneNumber}").SetFontSize(10));
                    document.Add(new Paragraph($"Relação de Parentesco: {contract.AlternativeContact.Relationship}").SetFontSize(10));
                    document.Add(new Paragraph("\n"));
                }

                // Property info
                var area = contract.ContractPlots.First().Plot.Area;
                document.Add(new Paragraph("INFORMAÇÕES DO IMÓVEL").SetBold().SetFontSize(12));
                document.Add(new Paragraph($"Categoria: {area.Name}").SetFontSize(10));
                document.Add(new Paragraph($"Dimensões: {area.Dimensions}").SetFontSize(10));
                document.Add(new Paragraph($"Padrão Habitacional: {area.HousingStandard}").SetFontSize(10));
                document.Add(new Paragraph("\n"));

                // Selected plots
                document.Add(new Paragraph("TERRENOS SELECIONADOS").SetBold().SetFontSize(12));
                var plotNumbers = string.Join(", ", 
                    contract.ContractPlots.OrderBy(cp => int.Parse(cp.Plot.PlotNumber))
                        .Select(cp => cp.Plot.PlotNumber));
                document.Add(new Paragraph($"Terreno(s): {plotNumbers}").SetFontSize(10));
                document.Add(new Paragraph($"Total de Terrenos: {contract.TotalPlots}").SetFontSize(10));
                document.Add(new Paragraph("\n"));

                // Financial info
                document.Add(new Paragraph("INFORMAÇÕES FINANCEIRAS").SetBold().SetFontSize(12));
                document.Add(new Paragraph($"Valor de Adesão (Total): {contract.TotalAdhesionValue:N2} MT").SetFontSize(10));
                document.Add(new Paragraph($"Prestação Mensal (Unitária): {area.MonthlyInstallment:N2} MT").SetFontSize(10));
                document.Add(new Paragraph($"Prazo de Pagamento: {contract.PaymentPeriodMonths} meses").SetFontSize(10));
                document.Add(new Paragraph($"Valor Total das Mensalidades: {contract.TotalInstallmentValue:N2} MT").SetFontSize(10));
                document.Add(new Paragraph("\n"));

                // Legal notice
                document.Add(new Paragraph("CLÁUSULAS E CONDIÇÕES").SetBold().SetFontSize(12));
                document.Add(new Paragraph(
                    "1. O cliente declara que aceita os termos e condições de aquisição dos terrenos acima descriminados. " +
                    "2. O valor da adesão deve ser pago conforme comprovativo anexado. " +
                    "3. As mensalidades deverão ser pagas mensalmente conforme calendário de amortização. " +
                    "4. O cliente concorda em cumprir com todas as regulamentações e normas da Cooperativa de Habitação Novo Horizonte. " +
                    "5. Este contrato é válido mediante aprovação da documentação por parte do operador comercial."
                ).SetFontSize(9));
                document.Add(new Paragraph("\n"));

                // Signatures
                document.Add(new Paragraph("ASSINATURAS E APROVAÇÃO").SetBold().SetFontSize(12));
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("Cliente: ________________________  Data: __/__/____").SetFontSize(10));
                document.Add(new Paragraph(contract.Client.FullName).SetFontSize(9).SetItalic());
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("Operador Comercial: ________________________  Data: __/__/____").SetFontSize(10));
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("Administrador: ________________________  Data: __/__/____").SetFontSize(10));

                document.Close();

                return memoryStream.ToArray();
            }
        }

        public async Task<string> SaveContractPdfAsync(int contractId, string savePath)
        {
            var pdfBytes = await GenerateContractPdfAsync(contractId);
            var fileName = $"Contrato_{contractId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(savePath, fileName);

            Directory.CreateDirectory(savePath);
            await File.WriteAllBytesAsync(filePath, pdfBytes);

            return filePath;
        }
    }
}
