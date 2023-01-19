﻿using Orders.com.BLL.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Orders.com.WPF.VM
{
    public class OrdersVM : ViewModelBase, IListener<OrderUpdatedEvent>, IListener<OrderInsertedEvent>
    {
        private OrderService _ordersService;
        private ObservableCollection<OrderVM> _orders;
        private ICommand _loadOrdersCommand;
        private ICommand _deleteSelectedCommand;
        private EventAggregator _eventAggregator;
        private MainWindowVM _mainVM;

        public OrdersVM(OrderService categoryService, MainWindowVM mainVM, EventAggregator eventAggregator)
        {
            _ordersService = categoryService;
            _mainVM = mainVM;
            _loadOrdersCommand = new Command(async () => await LoadOrdersAsync());
            _deleteSelectedCommand = new Command(async () => await DeleteSelectedItemAsync());
            _eventAggregator = eventAggregator;
            _eventAggregator.AddListener<OrderInsertedEvent>(this, true);
        }

        public OrderVM SelectedOrder
        {
            get;
            set;
        }

        public ICommand LoadOrdersCommand
        {
            get { return _loadOrdersCommand; }
        }

        public ICommand DeleteSelectedCommand
        {
            get { return _deleteSelectedCommand; }
        }

        public IEnumerable<OrderVM> Orders
        {
            get { return _orders; }
            set
            {
                _orders = new ObservableCollection<OrderVM>(value);
                OnPropertyChanged("Orders");
            }
        }

        private async Task LoadOrdersAsync()
        {
            var result = await _ordersService.GetAllCommand(0, 100).ExecuteAsync();
            var vms = result.Value.Select(c => new OrderVM(c, _ordersService));
            Orders = vms.ToArray();
        }

        private async Task DeleteSelectedItemAsync()
        {
            var result = await _ordersService.DeleteCommand(SelectedOrder.ID).ExecuteAsync();
            if (result.Success)
            {
                _orders.Remove(SelectedOrder);
                SelectedOrder = null;
            }
        }

        public void Handle(OrderUpdatedEvent message)
        {
            var updatedOrder = message.Order;
            var order = Orders.First(o => o.ID == message.Order.ID);
            order.CustomerID = updatedOrder.CurrentCustomerID;
            order.Customer = updatedOrder.Customers.First(c => c.ID == order.CustomerID).Name;
            order.Total = updatedOrder.OrderItems.Sum(i => i.Amount.Value);
            order.Status = updatedOrder.Status == null ? string.Empty : updatedOrder.Status.Name;
            LoadOrdersAsync();
        }

        public void Handle(OrderInsertedEvent message)
        {
            _orders.Add(new OrderVM(message.Order, _mainVM, _ordersService));
        }
    }
}
