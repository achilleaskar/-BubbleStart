﻿using BubbleStart.Database;
using BubbleStart.Model;
using BubbleStart.Views;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace BubbleStart.ViewModels
{
    public class SearchCustomer_ViewModel : MyViewModelBase
    {
        #region Constructors

        public SearchCustomer_ViewModel()
        {

        }
        public SearchCustomer_ViewModel(GenericRepository context)
        {
            Context = context;
            CreateNewCustomerCommand = new RelayCommand(CreateNewCustomer);
            SaveCustomerCommand = new RelayCommand(async () => { await SaveCustomer(); }, CanSaveCustomer);
            ShowedUpCommand = new RelayCommand(async () => { await CustomerShowedUp(); });
            CustomerLeftCommand = new RelayCommand(async () => { await CustomerLeft(); });
            BodyPartSelected = new RelayCommand<string>(BodyPartChanged);
            CustomersPracticing = new ObservableCollection<Customer>();

            //PaymentCommand.CanExecute((obj) => CanMakePayment(obj));
            // CreateNewCustomer();
        }

        private void Customers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Customer customer in e.OldItems)
                {
                    //Removed items
                    customer.PropertyChanged -= EntityViewModelPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {

                foreach (Customer customer in e.NewItems)
                {

                    customer.PropertyChanged += EntityViewModelPropertyChanged;
                }
            }
        }

        private void EntityViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(SelectedCustomer));
        }

        public bool Enabled => SelectedCustomer != null;

        private bool CanSaveCustomer()
        {
            return SelectedCustomer != null && !string.IsNullOrEmpty(SelectedCustomer.Name) && !string.IsNullOrEmpty(SelectedCustomer.SureName) && !string.IsNullOrEmpty(SelectedCustomer.Tel);
        }

        #endregion Constructors

        #region Fields

        private ObservableCollection<Customer> _Customers;

        private ObservableCollection<Customer> _CustomersPracticing;

        private string _SearchTerm;

        private Customer _SelectedCustomer;

        private Customer _SelectedPracticingCustomer;

        #endregion Fields

        #region Properties

        public RelayCommand<string> BodyPartSelected { get; set; }

        public GenericRepository Context { get; }

        public RelayCommand CreateNewCustomerCommand { get; set; }

        public RelayCommand CustomerLeftCommand { get; set; }

        public ObservableCollection<Customer> Customers
        {
            get
            {
                return _Customers;
            }

            set
            {
                if (_Customers == value)
                {
                    return;
                }

                _Customers = value;
                Customers.CollectionChanged += Customers_CollectionChanged;

                RaisePropertyChanged();
            }
        }

        private ICollectionView _CustomersCollectionView;

        public ICollectionView CustomersCollectionView
        {
            get
            {
                return _CustomersCollectionView;
            }

            set
            {
                if (_CustomersCollectionView == value)
                {
                    return;
                }

                _CustomersCollectionView = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Customer> CustomersPracticing
        {
            get
            {
                return _CustomersPracticing;
            }

            set
            {
                if (_CustomersPracticing == value)
                {
                    return;
                }

                _CustomersPracticing = value;
                CustomersPracticingCollectionView = CollectionViewSource.GetDefaultView(CustomersPracticing);
                CustomersPracticingCollectionView.SortDescriptions.Add(new SortDescription(nameof(Customer.SureName), ListSortDirection.Ascending));
                RaisePropertyChanged();
            }
        }

        public ICollectionView CustomersPracticingCollectionView { get; set; }

        public RelayCommand SaveCustomerCommand { get; set; }

        public string SearchTerm
        {
            get
            {
                return _SearchTerm;
            }

            set
            {
                if (_SearchTerm == value)
                {
                    return;
                }
                _SearchTerm = value;
                if (CustomersCollectionView != null)
                    CustomersCollectionView.Refresh();
                RaisePropertyChanged();
            }
        }

        public Customer SelectedCustomer
        {
            get
            {
                return _SelectedCustomer;
            }

            set
            {
                if (_SelectedCustomer == value)
                {
                    return;
                }
                if (_SelectedCustomer != null)
                {
                    _SelectedCustomer.IsSelected = false;
                }
                _SelectedCustomer = value;
                if (_SelectedCustomer != null)
                {
                    _SelectedCustomer.SelectProperProgram();
                }
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Enabled));
            }
        }

        public Customer SelectedPracticingCustomer
        {
            get
            {
                return _SelectedPracticingCustomer;
            }

            set
            {
                if (_SelectedPracticingCustomer == value)
                {
                    return;
                }

                _SelectedPracticingCustomer = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ShowedUpCommand { get; set; }

        #endregion Properties

        #region Methods

        public async Task CustomerLeft()
        {
            SelectedPracticingCustomer.LastShowUp.Left = DateTime.Now;
            SelectedPracticingCustomer.IsPracticing = false;
            CustomersPracticing.Remove(SelectedPracticingCustomer);
            await Context.SaveAsync();
        }

        public override async Task LoadAsync(int id = 0, MyViewModelBase previousViewModel = null)
        {
            List<District> Districts = (await Context.GetAllAsync<District>()).OrderBy(d => d.Name).ToList();
            Helpers.StaticResources.Districts.Clear();
            foreach (var item in Districts)
            {
                Helpers.StaticResources.Districts.Add(item);
            }
            OpenCustomerManagementCommand = new RelayCommand(OpenCustomerManagement);
            Customers = new ObservableCollection<Customer>((await Context.LoadAllCustomersAsync()).OrderByDescending(c => c.ActiveCustomer).ThenBy(x => x.SureName));
            CustomersCollectionView = CollectionViewSource.GetDefaultView(Customers);
            CustomersCollectionView.Filter = CustomerFilter;

            foreach (var item in Customers)
            {
                item.SelectProperProgram();
                if (item.LastShowUp != null && item.LastShowUp.Left < item.LastShowUp.Arrived && item.LastShowUp.Left.Year != 1234)
                {
                    item.IsPracticing = true;
                    CustomersPracticing.Add(item);
                }
            }
            foreach (var c in Customers)
            {
                c.CalculateRemainingAmount();
            }

        }



        private void OpenCustomerManagement()
        {
            if (SelectedCustomer != null)
            {
                if (Context.HasChanges())
                {
                    MessageBoxResult result = MessageBox.Show("Υπάρχουν μη αποθηκευμένες αλλαγές, θέλετε σίγουρα να συνεχίσετε?", "Προσοχή", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                if (Context.HasChanges())
                    Context.RollBack();
                SelectedCustomer.Context = Context;
                Window window = new CustomerManagement
                {
                    DataContext = SelectedCustomer
                };
                Application.Current.MainWindow.Visibility = Visibility.Hidden;
                window.ShowDialog();
                if (Context.HasChanges())
                    Context.RollBack();
                Application.Current.MainWindow.Visibility = Visibility.Visible;
            }
        }

        public RelayCommand OpenCustomerManagementCommand { get; set; }

        public override Task ReloadAsync()
        {
            throw new NotImplementedException();
        }

        private void BodyPartChanged(string selectedIndex)
        {
            _SelectedCustomer.Illness.SelectedIllnessPropertyName = selectedIndex;
        }

        private void CreateNewCustomer()
        {
            SelectedCustomer = new Customer();
        }

        private bool CustomerFilter(object item)
        {
            Customer customer = item as Customer;
            return string.IsNullOrEmpty(SearchTerm) || customer.Name.ToLower().Contains(SearchTerm) || customer.SureName.ToLower().Contains(SearchTerm) || customer.Tel.Contains(SearchTerm);
        }

        public async Task CustomerShowedUp()
        {
            if (SelectedCustomer != null)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                CustomersPracticing.Add(SelectedCustomer);
                SelectedCustomer.ShowedUp(true);
                await Context.SaveAsync();
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private async Task SaveCustomer()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (SelectedCustomer != null)
            {
                //if (SelectedCustomer.NewWeight > 0)
                //{
                //    SelectedCustomer.WeightHistory.Add(new Weight { WeightValue = SelectedCustomer.NewWeight });
                //    SelectedCustomer.NewWeight = 0;
                //}
                if (!string.IsNullOrEmpty(SelectedCustomer.DistrictText) && !Helpers.StaticResources.Districts.Any(d => d.Name == SelectedCustomer.DistrictText))
                {
                    Context.Add(new District { Name = SelectedCustomer.DistrictText });
                }
                if (SelectedCustomer.Id > 0)
                {
                    await Context.SaveAsync();
                }
                else
                {
                    Context.Add(SelectedCustomer);
                    Customers.Add(SelectedCustomer);
                    await Context.SaveAsync();
                }
                SelectedCustomer.RaisePropertyChanged(nameof(Customer.BMI));
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        #endregion Methods
    }

    public class CustomSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            int digitsX = x.ToString().Length;
            int digitsY = y.ToString().Length;
            if (digitsX < digitsY)
            {
                return 1;
            }
            else if (digitsX > digitsY)
            {
                return -1;
            }
            return (int)x - (int)y;
        }
    }
}