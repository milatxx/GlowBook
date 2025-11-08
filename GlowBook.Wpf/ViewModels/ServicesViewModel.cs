using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using GlowBook.Wpf.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;


namespace GlowBook.Wpf.ViewModels
{
    public class ServicesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Service> Items { get; } = new();
        private Service? _selectedService;
        public Service? SelectedService
        {
            get => _selectedService;
            set { _selectedService = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }
        private AppDbContext NewDb() => App.Services.GetRequiredService<AppDbContext>();

        public ICommand ReloadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public ServicesViewModel()
        {
            

            ReloadCommand = new RelayCommand(_ => Load());
            AddCommand = new RelayCommand(_ => Add());
            SaveCommand = new RelayCommand(_ => Save(SelectedService!), _ => SelectedService != null);
            DeleteCommand = new RelayCommand(_ =>
            {
                if (SelectedService != null)
                {
                    SoftDelete(SelectedService);
                    Items.Remove(SelectedService);
                    SelectedService = null;
                }
            }, _ => SelectedService != null);

            Load();
        }


        public void Load()
    {
        Items.Clear();
        var db = NewDb();
        foreach (var s in db.Services.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.Name))
            Items.Add(s);
            SelectedService = Items.FirstOrDefault();
        }

    public void Save(Service s)
    {
        var db = NewDb();
        if (s.Id == 0) db.Services.Add(s); else db.Services.Update(s);
        db.SaveChanges();
    }

    public void SoftDelete(Service s)
    {
        var db = NewDb();
        var entity = db.Services.First(x => x.Id == s.Id);
        entity.IsDeleted = true;
        db.SaveChanges();
    }
        // Add a new service
        private void Add()
        {
            var s = new Service { Name = "Nieuwe dienst", DurationMin = 30, Price = 0m };
            Items.Add(s);
            SelectedService = s;
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // RelayCommand implementation
        private sealed class RelayCommand : ICommand
        {
            private readonly Action<object?> _execute;
            private readonly Predicate<object?>? _canExecute;
            public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
            {
                _execute = execute; _canExecute = canExecute;
            }
            public bool CanExecute(object? p) => _canExecute?.Invoke(p) ?? true;
            public void Execute(object? p) => _execute(p);
            public event EventHandler? CanExecuteChanged
            { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }
        }
    }
}
