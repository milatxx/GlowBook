using GlowBook.Model.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using GlowBook.Wpf.Helpers;
using GlowBook.Wpf.Services;
using GlowBook.Wpf.Views.Pages;

namespace GlowBook.Wpf.ViewModels
{
    public class HomeViewModel
    {
        public ObservableCollection<(string Time, string Customer, string Service)> TodayAppointments { get; } = new();
        public int CustomerCount { get; private set; }
        public string TopServiceName { get; private set; } = "";
        public int TopServiceCount { get; private set; }
        public decimal RevenueThisWeek { get; private set; }

        public RelayCommand OpenCustomersCommand { get; } = null!;
        public RelayCommand OpenReportsCommand { get; } = null!;

        public HomeViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;
            var db = App.Services.GetRequiredService<AppDbContext>();
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var appts = db.Appointments
                .Include(a => a.Customer)
                .Include(a => a.AppointmentServices).ThenInclude(x => x.Service)
                .Where(a => a.Start >= today && a.Start < tomorrow)
                .OrderBy(a => a.Start).ToList();

            foreach (var a in appts)
            {
                var svc = a.AppointmentServices.Select(s => s.Service.Name).FirstOrDefault() ?? "-";
                TodayAppointments.Add((a.Start.ToString("HH:mm"), a.Customer.Name, svc));
            }

            CustomerCount = db.Customers.Count();

            var monthStart = new DateTime(today.Year, today.Month, 1);
            var top = db.AppointmentServices
                .Include(x => x.Service)
                .Include(x => x.Appointment)
                .Where(x => x.Appointment.Start >= monthStart)
                .GroupBy(x => x.Service.Name)
                .Select(g => new { Name = g.Key, C = g.Count() })
                .OrderByDescending(x => x.C).FirstOrDefault();

            if (top != null) { TopServiceName = top.Name; TopServiceCount = top.C; }

            var weekStart = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            RevenueThisWeek = db.AppointmentServices
                .Include(x => x.Service).Include(x => x.Appointment)
                .Where(x => x.Appointment.Start >= weekStart)
                .Sum(x => x.Service.Price * x.Qty);

            OpenCustomersCommand = new RelayCommand(OpenCustomers);
            OpenReportsCommand = new RelayCommand(OpenReports);
        }


        private void OpenCustomers(object? parameter)
        {
            GlowBook.Wpf.Helpers.NavigationHelper.Navigate(new GlowBook.Wpf.Views.Pages.CustomersPage());
        }

        private void OpenReports(object? parameter)
        {
            GlowBook.Wpf.Helpers.NavigationHelper.Navigate(new GlowBook.Wpf.Views.Pages.ReportsPage());
        }


    }
}
