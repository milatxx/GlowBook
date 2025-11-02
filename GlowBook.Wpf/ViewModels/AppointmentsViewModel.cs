using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GlowBook.Wpf.ViewModels
{
    public class AppointmentsViewModel
        {
        private AppDbContext NewDb() => App.Services.GetRequiredService<AppDbContext>();

        public ObservableCollection<Appointment> Items { get; } = new();
        public Appointment? Selected { get; set; }

        public DateTime From { get; set; } = DateTime.Today;
        public DateTime To { get; set; } = DateTime.Today.AddDays(7);

        public void Load()
        {
            Items.Clear();
            using var db = NewDb();

            var data = db.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Staff)
                .Include(a => a.AppointmentServices).ThenInclude(x => x.Service)
                .Where(a => !a.IsDeleted && a.Start >= From && a.Start < To)
                .OrderBy(a => a.Start)
                .ToList();

            foreach (var a in data) Items.Add(a);
            Selected = Items.FirstOrDefault();
        }
    }
}
