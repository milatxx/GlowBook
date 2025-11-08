using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using GlowBook.Wpf.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace GlowBook.Wpf.ViewModels
{
     public class CustomersViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<Customer> Items { get; } = new();

        private Customer? _selectedCustomer;
        public Customer? SelectedCustomer

        {
            get => _selectedCustomer;
            set { _selectedCustomer = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        public ICommand ReloadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public CustomersViewModel()
        {
            ReloadCommand = new RelayCommand(_ => Load());
            AddCommand = new RelayCommand(_ => Add());
            SaveCommand = new RelayCommand(_ =>
            {
                if (SelectedCustomer != null) Save(SelectedCustomer);
            });
            DeleteCommand = new RelayCommand(_ =>
            {
                if (SelectedCustomer != null)
                {
                    SoftDelete(SelectedCustomer);
                    Items.Remove(SelectedCustomer);
                    SelectedCustomer = null;
                }
            }, _ => SelectedCustomer != null);

            Load();
        }

        private AppDbContext NewDb() => App.Services.GetRequiredService<AppDbContext>();

        public void Load()
        {
            Items.Clear();
            var db = NewDb();
            var data = db.Customers.AsNoTracking()
                                   .Where(c => !c.IsDeleted)
                                   .OrderBy(c => c.Name)
                                   .ToList();
            foreach (var c in data) Items.Add(c);

            SelectedCustomer = Items.FirstOrDefault();
        }

        public void Save(Customer c)
        {
            var db = NewDb();
            if (c.Id == 0) db.Customers.Add(c);
            else db.Customers.Update(c);
            db.SaveChanges();
        }

        // Softdelete
        public void SoftDelete(Customer c)
        {
            var db = NewDb();
            var entity = db.Customers.First(x => x.Id == c.Id);
            entity.IsDeleted = true;
            db.SaveChanges();
        }

        private void Add()
        {
            var c = new Customer { Name = "Nieuwe klant" };
            Items.Add(c);
            SelectedCustomer = c;
        }

        
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        
        private sealed class RelayCommand : ICommand
        {
            private readonly Action<object?> _execute;
            private readonly Predicate<object?>? _canExecute;

            public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
            public void Execute(object? parameter) => _execute(parameter);

            public event EventHandler? CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }
        }
    }
}
