﻿using Peasy;
using Peasy.Extensions;
using Orders.com.BLL.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Orders.com.BLL.Domain;
using Orders.com.BLL.Services;

namespace Orders.com.WPF.VM
{
    public class CustomerOrderVM : OrdersDotComVMBase<Order>
    {
        private OrderItemService _orderItemService;
        private OrderService _orderService;
        private ObservableCollection<OrderItemVM> _orderItems;
        private System.Windows.Input.ICommand _saveOrderCommand;
        private System.Windows.Input.ICommand _submitOrderCommand;
        private System.Windows.Input.ICommand _shipOrderCommand;
        private System.Windows.Input.ICommand _addOrderItemCommand;
        private System.Windows.Input.ICommand _deleteSelectedItemCommand;
        private System.Windows.Input.ICommand _refreshCommand;
        private EventAggregator _eventAggregator;
        private MainWindowVM _mainVM;
        private InventoryItemService _inventoryService;

        public CustomerOrderVM(EventAggregator eventAggregator, OrderService orderService, OrderItemService orderItemService, InventoryItemService inventoryService, MainWindowVM mainVM)
            : base(orderService)
        {
            Setup(eventAggregator, orderService, orderItemService, inventoryService, mainVM);
        }

        public CustomerOrderVM(EventAggregator eventAggregator, Order order, OrderService orderService, OrderItemService orderItemService, InventoryItemService inventoryService, MainWindowVM mainVM)
            : base(order, orderService)
        {
            Setup(eventAggregator, orderService, orderItemService, inventoryService, mainVM);
        }

        private void Setup(EventAggregator eventAggregator, OrderService orderService, OrderItemService orderItemService, InventoryItemService inventoryService, MainWindowVM mainVM)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
            _inventoryService = inventoryService;
            _mainVM = mainVM;
            _eventAggregator = eventAggregator;
            _orderItems = new ObservableCollection<OrderItemVM>();
            _saveOrderCommand = new Command(async () => await SaveAsync());
            _addOrderItemCommand = new Command(() => AddOrderItem(), () => CanAdd);
            _deleteSelectedItemCommand = new Command(async () => await DeleteSelectedItemAsync());
            _submitOrderCommand = new Command(async () => await SubmitAsync());
            _shipOrderCommand = new Command(async () => await ShipAsync());
            _refreshCommand = new Command(async () => await LoadOrderItemsAsync());
        }

        public IEnumerable<CustomerVM> Customers
        {
            get { return _mainVM.CustomersVM.Customers; }
        }

        public OrderItemVM SelectedOrderItem
        {
            get;
            set;
        }

        public long ID
        {
            get { return CurrentEntity.OrderID; }
        }

        public decimal Total
        {
            get
            {
                if (!OrderItems.Any()) return 0;
                return OrderItems.Sum(o => o.Amount.GetValueOrDefault());
            }
        }

        public long CurrentCustomerID
        {
            get { return CurrentEntity.CustomerID; }
            set
            {
                CurrentEntity.CustomerID = value;
                IsDirty = true;
                OnPropertyChanged("CurrentCustomerID", "CanSave");
            }
        }

        public OrderStateBase Status
        {
            get { return OrderItems.OrderStatus(); }
        }

        public IEnumerable<OrderItemVM> OrderItems
        {
            get { return _orderItems; }
        }

        public System.Windows.Input.ICommand AddOrderItemCommand
        {
            get { return _addOrderItemCommand; }
        }

        public System.Windows.Input.ICommand SaveOrderCommand
        {
            get { return _saveOrderCommand; }
        }

        public System.Windows.Input.ICommand SubmitOrderCommand
        {
            get { return _submitOrderCommand; }
        }

        public System.Windows.Input.ICommand ShipOrderCommand
        {
            get { return _shipOrderCommand; }
        }

        public System.Windows.Input.ICommand DeleteSelectedItemCommand
        {
            get { return _deleteSelectedItemCommand; }
        }

        public System.Windows.Input.ICommand RefreshCommand
        {
            get { return _refreshCommand; }
        }

        protected override void OnInsertSuccess(Order result)
        {
            OnPropertyChanged("ID");
            _eventAggregator.SendMessage<OrderInsertedEvent>(new OrderInsertedEvent { Order = this });
        }

        protected override void OnUpdateSuccess(Order result)
        {
            _eventAggregator.SendMessage<OrderUpdatedEvent>(new OrderUpdatedEvent(this));
        }

        public bool CanChangeCustomer
        {
            get { return OrderItems.All(i => i.Status is ShippedState == false); }
        }

        public bool CanAdd
        {
            get { return !IsNew || IsDirty; }
        }

        public override bool CanSave
        {
            get { return IsDirty || OrderItems.Any(i => i.IsDirty); }
        }

        public bool CanSubmit
        {
            get
            {
                return OrderItems.Any(i => i.Status == null || i.Status.CanSubmit) &&
                       OrderItems.All(i => (!i.IsDirty && !i.IsNew));
            }
        }

        public bool CanShip
        {
            get
            {
                return OrderItems.Any(i => i.Status == null || i.Status.CanShip) &&
                        OrderItems.All(i => (!i.IsDirty && !i.IsNew));
            }
        }

        public override async Task SaveAsync()
        {
            if (CanSave)
            {
                await base.SaveAsync();
                var results = OrderItems.ForEach(item => item.OrderID = CurrentEntity.OrderID)
                                        .Select(vm => vm.SaveAsync())
                                        .ToArray();
                await Task.WhenAll(results);
                OnPropertyChanged("CanSave", "CanSubmit", "CanShip");
            }
        }

        public async Task SubmitAsync()
        {
            if (CanSubmit)
            {
                var submitTasks = OrderItems.Select(i => i.SubmitAsync()).ToArray();
                await Task.WhenAll(submitTasks);
                _eventAggregator.SendMessage<OrderUpdatedEvent>(new OrderUpdatedEvent(this));
                OnPropertyChanged("CanSubmit");
            }
        }

        public async Task ShipAsync()
        {
            if (CanShip)
            {
                var shipTasks = OrderItems.Select(i => i.SubmitAsync()).ToArray();
                await Task.WhenAll(shipTasks);
                _eventAggregator.SendMessage<OrderUpdatedEvent>(new OrderUpdatedEvent(this));
                OnPropertyChanged("CanShip", "CanChangeCustomer");
            }
        }

        private void AddOrderItem()
        {
            if (CanAdd)
            {
                var item = new OrderItemVM(_orderItemService, _inventoryService, _mainVM);
                SubscribeHandlers(item);
                _orderItems.Add(item);
            }
        }

        private async Task LoadOrderItemsAsync()
        {
            var result = await _orderItemService.GetByOrderCommand(CurrentEntity.OrderID).ExecuteAsync();
            _orderItems.Clear();
            result.Value.ForEach(i => LoadOrderItem(i));
            OnPropertyChanged("CurrentCustomerID", "CanSave", "CanSubmit", "CanShip", "CanChangeCustomer", "Total");
        }

        private void LoadOrderItem(OrderItem orderItem)
        {
            var item = new OrderItemVM(orderItem, _orderItemService, _inventoryService, _mainVM);
            SubscribeHandlers(item);
            _orderItems.Add(item);
        }

        private void SubscribeHandlers(OrderItemVM item)
        {
            item.EntitySaved += (s, e) =>
            {
                _eventAggregator.SendMessage<OrderUpdatedEvent>(new OrderUpdatedEvent(this));
            };
            item.EntityDeleted += (s, e) =>
            {
                _orderItems.Remove(SelectedOrderItem);
                OnPropertyChanged("Total", "CanChangeCustomer");
                _eventAggregator.SendMessage<OrderUpdatedEvent>(new OrderUpdatedEvent(this));
            };
            item.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged("Total", "CanSave");
                if (e.PropertyName == "ShippedOn")
                {
                    _eventAggregator.SendMessage<OrderUpdatedEvent>(new OrderUpdatedEvent(this));
                    OnPropertyChanged("CanChangeCustomer");
                }
            };
        }

        private async Task DeleteSelectedItemAsync()
        {
            if (SelectedOrderItem.IsNew)
                _orderItems.Remove(SelectedOrderItem);
            else
                await SelectedOrderItem.DeleteAsync();
            OnPropertyChanged("CanSubmit", "CanShip");
        }
    }
}
