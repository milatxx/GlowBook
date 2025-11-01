using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;


namespace GlowBook.Wpf.ViewModels
{
    public class ServicesViewModel
    {
        public ObservableCollection<Service> Items { get; } = new();
    private AppDbContext NewDb() => App.Services.GetRequiredService<AppDbContext>();

    public ServicesViewModel() { Load(); }

    public void Load()
    {
        Items.Clear();
        using var db = NewDb();
        foreach (var s in db.Services.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.Name))
            Items.Add(s);
    }

    public void Save(Service s)
    {
        using var db = NewDb();
        if (s.Id == 0) db.Services.Add(s); else db.Services.Update(s);
        db.SaveChanges();
    }

    public void SoftDelete(Service s)
    {
        using var db = NewDb();
        var entity = db.Services.First(x => x.Id == s.Id);
        entity.IsDeleted = true;
        db.SaveChanges();
    }
}
}
