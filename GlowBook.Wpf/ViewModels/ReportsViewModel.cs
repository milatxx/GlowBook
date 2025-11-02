using GlowBook.Model.Data;
using Microsoft.Extensions.DependencyInjection;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;

namespace GlowBook.Wpf.ViewModels
{
    public class ReportsViewModel
    {
        public DateTime From { get; set; } = DateTime.Today.AddDays(-7);
        public DateTime To { get; set; } = DateTime.Today;

        public decimal RevenueTotal { get; private set; }

        public void Calculate()
        {
            using var db = App.Services.GetRequiredService<AppDbContext>();
            var q = db.AppointmentServices.Include(x => x.Service)
                    .Include(x => x.Appointment)
                    .Where(x => x.Appointment.Start >= From && x.Appointment.Start < To.AddDays(1));

            RevenueTotal = q.Sum(x => x.Service.Price * x.Qty);
        }

        public string ExportCsv(string path)
        {
            using var db = App.Services.GetRequiredService<AppDbContext>();
            var q = db.AppointmentServices.Include(x => x.Service).Include(x => x.Appointment)
                    .Include(x => x.Appointment.Customer)
                    .Where(x => x.Appointment.Start >= From && x.Appointment.Start < To.AddDays(1))
                    .OrderBy(x => x.Appointment.Start)
                    .Select(x => new {
                        Date = x.Appointment.Start,
                        Customer = x.Appointment.Customer.Name,
                        Service = x.Service.Name,
                        Price = x.Service.Price,
                        Qty = x.Qty
                    }).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Date,Customer,Service,Price,Qty,Total");
            foreach (var r in q)
            {
                var tot = r.Price * r.Qty;
                sb.AppendLine($"{r.Date:yyyy-MM-dd HH:mm},{r.Customer},{r.Service},{r.Price},{r.Qty},{tot}");
            }
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            return path;
        }

        public string ExportPdf(string path)
        {
            Calculate();
            var doc = new PdfDocument();
            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Segoe UI", 18, XFontStyle.Bold);
            var small = new XFont("Segoe UI", 12, XFontStyle.Regular);

            gfx.DrawString("GlowBook Rapport", font, XBrushes.Black, new XRect(30, 30, page.Width, 30));
            gfx.DrawString($"Periode: {From:dd/MM/yyyy} - {To:dd/MM/yyyy}", small, XBrushes.Black, 30, 70);
            gfx.DrawString($"Omzet: € {RevenueTotal:N2}", small, XBrushes.Black, 30, 90);

            doc.Save(path);
            return path;
        }
    }
}
