using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GlowBook.Wpf.ViewModels
{
     public class CustomersViewModel
    {
        public ObservableCollection<Customer> Items { get; } = new();

        public CustomersViewModel()
        {
            Load();
        }

        private AppDbContext NewDb() => App.Services.GetRequiredService<AppDbContext>();

        public void Load()
        {
            Items.Clear();
            using var db = NewDb();
            var data = db.Customers.AsNoTracking()
                                   .Where(c => !c.IsDeleted)
                                   .OrderBy(c => c.Name)
                                   .ToList();
            foreach (var c in data) Items.Add(c);
        }

        public void Save(Customer c)
        {
            using var db = NewDb();
            if (c.Id == 0) db.Customers.Add(c);
            else db.Customers.Update(c);
            db.SaveChanges();
        }

        // Softdelete
        public void SoftDelete(Customer c)
        {
            using var db = NewDb();
            var entity = db.Customers.First(x => x.Id == c.Id);
            entity.IsDeleted = true;
            db.SaveChanges();
        }
    }
}
