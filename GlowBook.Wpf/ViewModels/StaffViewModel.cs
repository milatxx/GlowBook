using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GlowBook.Wpf.ViewModels
{
    public class StaffViewModel
    {
        public ObservableCollection<Staff> Items { get; } = new();
        private AppDbContext NewDb() => App.Services.GetRequiredService<AppDbContext>();

        public StaffViewModel() { Load(); }

        public void Load()
        {
            Items.Clear();
            using var db = NewDb();
            foreach (var s in db.Staff.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.Name))
                Items.Add(s);
        }

        public void Save(Staff s)
        {
            using var db = NewDb();
            if (s.Id == 0) db.Staff.Add(s); else db.Staff.Update(s);
            db.SaveChanges();
        }

        public void SoftDelete(Staff s)
        {
            using var db = NewDb();
            var entity = db.Staff.First(x => x.Id == s.Id);
            entity.IsDeleted = true;
            db.SaveChanges();
        }
    }
}
