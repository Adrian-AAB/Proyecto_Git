﻿using Peasy;
using Orders.com.BLL.Rules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Orders.com.BLL.Domain;
using Orders.com.BLL.DataProxy;
using Orders.com.BLL.Extensions;

namespace Orders.com.BLL.Commands
{
    public class ShipOrderItemCommand : Command<OrderItem>
    {
        private IOrderItemDataProxy _orderItemDataProxy;
        private long _orderItemID;
        private ITransactionContext _transactionContext;
        private IInventoryItemDataProxy _inventoryDataProxy;

        public ShipOrderItemCommand(long orderItemID, IOrderItemDataProxy orderItemDataProxy, IInventoryItemDataProxy inventoryDataProxy, ITransactionContext transactionContext)
        {
            _orderItemID = orderItemID;
            _orderItemDataProxy = orderItemDataProxy;
            _inventoryDataProxy = inventoryDataProxy;
            _transactionContext = transactionContext;
        }

        private OrderItem CurrentOrderItem { get; set; }

        protected override OrderItem OnExecute()
        {
            return _transactionContext.Execute(() =>
            {
                var inventoryItem = _inventoryDataProxy.GetByProduct(CurrentOrderItem.ProductID);
                if (inventoryItem.QuantityOnHand - CurrentOrderItem.Quantity >= 0)
                {
                    CurrentOrderItem.OrderStatus().SetShippedState();
                    CurrentOrderItem.ShippedDate = DateTime.Now.ToUniversalTime();
                    inventoryItem.QuantityOnHand -= CurrentOrderItem.Quantity;
                    _inventoryDataProxy.Update(inventoryItem);
                }
                else
                {
                    CurrentOrderItem.OrderStatus().SetBackorderedState();
                    CurrentOrderItem.BackorderedDate = DateTime.Now.ToUniversalTime();
                }
                return _orderItemDataProxy.Ship(CurrentOrderItem);
            });
        }

        protected override async Task<OrderItem> OnExecuteAsync()
        {
            return await _transactionContext.ExecuteAsync(async () =>
            {
                var inventoryItem = await _inventoryDataProxy.GetByProductAsync(CurrentOrderItem.ProductID);
                if (inventoryItem.QuantityOnHand - CurrentOrderItem.Quantity >= 0)
                {
                    CurrentOrderItem.OrderStatus().SetShippedState();
                    CurrentOrderItem.ShippedDate = DateTime.Now.ToUniversalTime();
                    inventoryItem.QuantityOnHand -= CurrentOrderItem.Quantity;
                    await _inventoryDataProxy.UpdateAsync(inventoryItem);
                }
                else
                {
                    CurrentOrderItem.OrderStatus().SetBackorderedState();
                    CurrentOrderItem.BackorderedDate = DateTime.Now.ToUniversalTime();
                }
                return await _orderItemDataProxy.ShipAsync(CurrentOrderItem);
            });
        }

        public IEnumerable<IRule> GetRules()
        {
            yield return new CanShipOrderItemRule(CurrentOrderItem);
        }

        public override IEnumerable<ValidationResult> GetErrors()
        {
            CurrentOrderItem = _orderItemDataProxy.GetByID(_orderItemID);
            foreach (var error in GetRules().GetValidationResults())
                yield return error;
        }

        public override async Task<IEnumerable<ValidationResult>> GetErrorsAsync()
        {
            CurrentOrderItem = await _orderItemDataProxy.GetByIDAsync(_orderItemID);
            return await GetRules().GetValidationResultsAsync();
        }
    }
}
