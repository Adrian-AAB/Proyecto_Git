﻿using Peasy;
using Orders.com.BLL.Commands;
using System.Collections.Generic;
using Orders.com.BLL.Domain;
using Orders.com.BLL.DataProxy;

namespace Orders.com.BLL.Services
{
    public class ProductService : OrdersDotComServiceBase<Product>, IProductService
    {
        private IInventoryItemService _inventoryService;
        private ITransactionContext _transactionContext;
        private IOrderDataProxy _orderDataProxy;

        public ProductService(IProductDataProxy dataProxy, IOrderDataProxy orderDataProxy, IInventoryItemService inventoryService, ITransactionContext transactionContext) : base(dataProxy)
        {
            _orderDataProxy = orderDataProxy;
            _inventoryService = inventoryService;
            _transactionContext = transactionContext;
        }

        public override ICommand<Product> InsertCommand(Product entity)
        {
            var dataProxy = DataProxy as IProductDataProxy;
            return new CreateProductCommand(entity, dataProxy, _inventoryService, _transactionContext);
        }

        public ICommand<IEnumerable<Product>> GetByCategoryCommand(long categoryID)
        {
            var dataProxy = DataProxy as IProductDataProxy;
            return new ServiceCommand<IEnumerable<Product>>
            (
                executeMethod: () => dataProxy.GetByCategory(categoryID),
                executeAsyncMethod: () => dataProxy.GetByCategoryAsync(categoryID)
            );
        }

        public override ICommand DeleteCommand(long id)
        {
            var dataProxy = DataProxy as IProductDataProxy;
            return new DeleteProductCommand(id, dataProxy, _inventoryService, _orderDataProxy, _transactionContext);
        }
    }
}
