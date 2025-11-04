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
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace GlowBook.Wpf.ViewModels
{
    public sealed class ServiceRevenueRow
    {
        public string Service { get; init; } = "";
        public decimal Revenue { get; init; }
    }

    public class ReportsViewModel : INotifyPropertyChanged
    {
        private DateTime _from = DateTime.Today.AddDays(-7);
        private DateTime _to = DateTime.Today;
        private decimal _revenueTotal;

        public DateTime From
        {
            get => _from;
            set { _from = value; OnPropertyChanged(); }
        }

        public DateTime To
        {
            get => _to;
            set { _to = value; OnPropertyChanged(); }
        }

        public decimal RevenueTotal
        {
            get => _revenueTotal;
            private set { _revenueTotal = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ServiceRevenueRow> RevenuePerService { get; } = new();
        public ObservableCollection<ServiceRevenueRow> RevenuePerService_MethodSyntax { get; } = new(); 


        private static DateTime EndOfDay(DateTime d) => d.Date.AddDays(1).AddTicks(-1);
        private AppDbContext Db() => App.Services.GetRequiredService<AppDbContext>();

        public void Calculate()
        {
            using var db = Db();

            var q = db.AppointmentServices
                .Include(x => x.Service)
                .Include(x => x.Appointment)
                .Where(x => !x.IsDeleted
                            && !x.Appointment.IsDeleted
                            && x.Appointment.Start >= From
                            && x.Appointment.Start <= EndOfDay(To));

            // voorkom InvalidOperation bij lege reeks
            RevenueTotal = q.Select(x => x.Service.Price * x.Qty).DefaultIfEmpty(0m).Sum();
        }

        public void RefreshRevenuePerService_QuerySyntax()
        {
            using var db = Db();

            // query syntax
            var q =
                from a in db.Appointments
                where a.Start >= From && a.Start <= EndOfDay(To) && a.Status == "Done"
                from link in a.AppointmentServices
                group link by link.Service.Name into g
                select new ServiceRevenueRow
                {
                    Service = g.Key,
                    Revenue = g.Sum(x => x.Qty * x.Service.Price)
                };

            var rows = q.OrderBy(r => r.Service).ToList();

            RevenuePerService.Clear();
            foreach (var r in rows)
                RevenuePerService.Add(r);
        }


        public string ExportCsv(string path)
        {
            using var db = Db();

            var rows = db.AppointmentServices
                .Include(x => x.Service)
                .Include(x => x.Appointment).ThenInclude(a => a.Customer)
                .Where(x => !x.IsDeleted
                            && !x.Appointment.IsDeleted
                            && x.Appointment.Start >= From
                            && x.Appointment.Start <= EndOfDay(To))
                .OrderBy(x => x.Appointment.Start)
                .Select(x => new
                {
                    Date = x.Appointment.Start,
                    Customer = x.Appointment.Customer.Name,
                    Service = x.Service.Name,
                    Price = x.Service.Price,
                    Qty = x.Qty,
                    Total = x.Service.Price * x.Qty
                })
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Date;Customer;Service;Price;Qty;Total");
            foreach (var r in rows)
            {
                sb.AppendLine($"{r.Date:yyyy-MM-dd HH\\:mm};{r.Customer};{r.Service};{r.Price.ToString("0.00", CultureInfo.InvariantCulture)};{r.Qty};{r.Total.ToString("0.00", CultureInfo.InvariantCulture)}");
            }

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            return path;
        }

        public string ExportPdf(string path)
        {
            // altijd eerst herberekenen
            Calculate();

            var doc = new PdfDocument();
            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var fontTitle = new XFont("Segoe UI", 16, XFontStyleEx.Bold);
            var font = new XFont("Segoe UI", 11, XFontStyleEx.Regular);

            double y = 40;
            gfx.DrawString("GlowBook Rapport", fontTitle, XBrushes.Black, 40, y); y += 20;
            gfx.DrawString($"Periode: {From:dd/MM/yyyy} - {To:dd/MM/yyyy}", font, XBrushes.Black, 40, y); y += 16;
            gfx.DrawString($"Omzet: € {RevenueTotal:N2}", font, XBrushes.Black, 40, y);

            doc.Save(path);
            return path;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

