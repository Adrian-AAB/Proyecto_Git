﻿using Peasy;
using Peasy.Extensions;
using Orders.com.BLL.Commands;
using Orders.com.BLL.Rules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orders.com.BLL.Domain;
using Orders.com.BLL.DataProxy;
using Orders.com.BLL.QueryData;

namespace Orders.com.BLL.Services
{
    public class OrderService : OrdersDotComServiceBase<Order>, IOrderService
    {
        private IOrderItemService _orderItemService;
        private ITransactionContext _transactionContext;

        public OrderService(IOrderDataProxy dataProxy, IOrderItemService orderItemService, ITransactionContext transactionContext) : base(dataProxy)
        {
            _orderItemService = orderItemService;
            _transactionContext = transactionContext;
        }

        private IOrderDataProxy OrdersDataProxy
        {
            get { return DataProxy as IOrderDataProxy; }
        }

        protected override void OnInsertCommandInitialization(Order entity, ExecutionContext<Order> context)
        {
            entity.OrderDate = DateTime.Now;
        }

        protected override async Task OnInsertCommandInitializationAsync(Order entity, ExecutionContext<Order> context)
        {
            OnInsertCommandInitialization(entity, context);
        }

        protected override IEnumerable<IRule> GetBusinessRulesForUpdate(Order entity, ExecutionContext<Order> context)
        {
            yield return new ValidOrderStatusForUpdateRule(entity.ID, _orderItemService);
        }

        protected override async Task<IEnumerable<IRule>> GetBusinessRulesForUpdateAsync(Order entity, ExecutionContext<Order> context)
        {
            return new ValidOrderStatusForUpdateRule(entity.ID, _orderItemService).ToArray();
        }

        public ICommand<IEnumerable<OrderInfo>> GetAllCommand(int start, int pageSize)
        {
            return new ServiceCommand<IEnumerable<OrderInfo>>
            (
                executeMethod: () => OrdersDataProxy.GetAll(start, pageSize),
                executeAsyncMethod: () => OrdersDataProxy.GetAllAsync(start, pageSize)
            );
        }

        public ICommand<IEnumerable<Order>> GetByCustomerCommand(long customerID)
        {
            return new ServiceCommand<IEnumerable<Order>>
            (
                executeMethod: () => OrdersDataProxy.GetByCustomer(customerID),
                executeAsyncMethod: () => OrdersDataProxy.GetByCustomerAsync(customerID)
            );
        }

        public override ICommand DeleteCommand(long id)
        {
            return new DeleteOrderCommand(id, OrdersDataProxy, _orderItemService, _transactionContext);
        }

        public ICommand<IEnumerable<Order>> GetByProductCommand(long productID)
        {
            return new ServiceCommand<IEnumerable<Order>>
            (
                executeMethod: () => OrdersDataProxy.GetByProduct(productID),
                executeAsyncMethod: () => OrdersDataProxy.GetByProductAsync(productID)
            );
        }
    }
}
