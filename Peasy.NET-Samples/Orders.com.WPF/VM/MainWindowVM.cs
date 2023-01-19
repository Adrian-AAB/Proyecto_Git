﻿using Orders.com.BLL.Services;

namespace Orders.com.WPF.VM
{
    public class MainWindowVM : ViewModelBase
    {
        private CustomersVM _customersVM;
        private ProductsVM _productsVM;
        private CategoriesVM _categoriesVM;
        private OrdersVM _ordersVM;
        private InventoryItemsVM _inventoryItemsVM;

        public MainWindowVM(EventAggregator eventAggregator,
                            CustomerService customerService,
                            ProductService productService,
                            CategoryService categoryService,
                            OrderService orderService,
                            InventoryItemService inventoryService)
        {
            _customersVM = new CustomersVM(customerService);
            _customersVM.LoadCustomersCommand.Execute(null);
            _productsVM = new ProductsVM(productService, this);
            _productsVM.LoadProductsCommand.Execute(null);
            _categoriesVM = new CategoriesVM(categoryService);
            _categoriesVM.LoadCategoriesCommand.Execute(null);
            _ordersVM = new OrdersVM(orderService, this, eventAggregator);
            _ordersVM.LoadOrdersCommand.Execute(null);
            _inventoryItemsVM = new InventoryItemsVM(inventoryService, this);
            _inventoryItemsVM.LoadInventoryCommand.Execute(null);
        }

        public CustomersVM CustomersVM
        {
            get { return _customersVM; }
        }

        public ProductsVM ProductsVM
        {
            get { return _productsVM; }
        }

        public CategoriesVM CategoriesVM
        {
            get { return _categoriesVM; }
        }

        public OrdersVM OrdersVM
        {
            get { return _ordersVM; }
        }

        public InventoryItemsVM InventoryItemsVM
        {
            get { return _inventoryItemsVM; }
        }
    }
}
