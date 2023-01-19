﻿using Peasy;
using Peasy.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Orders.com.BLL.DataProxy;
using Orders.com.BLL.Services;
using Orders.com.BLL.Domain;

namespace Orders.com.BLL.Commands
{
    public class DeleteOrderCommand : Command
    {
        private long _orderID;
        private IOrderDataProxy _orderDataProxy;
        private IOrderItemService _orderItemService;
        private ITransactionContext _transactionContext;

        public DeleteOrderCommand(long orderID, IOrderDataProxy orderDataProxy, IOrderItemService orderItemService, ITransactionContext transactionContext)
        {
            _orderID = orderID;
            _orderDataProxy = orderDataProxy;
            _orderItemService = orderItemService;
            _transactionContext = transactionContext;
        }

        private IEnumerable<OrderItem> CurrentOrderItems { get; set; }

        protected override void OnExecute()
        {
            _transactionContext.Execute(() =>
            {
                _orderDataProxy.Delete(_orderID);
                CurrentOrderItems.ForEach(i =>
                {
                    _orderItemService.DeleteCommand(i.ID).Execute();
                });
            });
        }

        protected override async Task OnExecuteAsync()
        {
            await _transactionContext.ExecuteAsync(async () =>
            {
                await _orderDataProxy.DeleteAsync(_orderID);
                foreach (var item in CurrentOrderItems)
                    await _orderItemService.DeleteCommand(item.ID).ExecuteAsync();
            });
        }

        public override IEnumerable<ValidationResult> GetErrors()
        {
            var rule = _orderID.CreateValueRequiredRule("order id").Validate();
            if (!rule.IsValid)
                yield return new ValidationResult(rule.ErrorMessage);
            else
            {
                CurrentOrderItems = _orderItemService.GetByOrderCommand(_orderID).Execute().Value;
                var errors = CurrentOrderItems.SelectMany(i => _orderItemService.DeleteCommand(i.ID).GetErrors());
                foreach (var error in errors)
                    yield return error;
            }
        }

        public override async Task<IEnumerable<ValidationResult>> GetErrorsAsync()
        {
            var results = new List<ValidationResult>();
            var rule = _orderID.CreateValueRequiredRule("order id").Validate();
            if (!rule.IsValid)
                results.Add(new ValidationResult(rule.ErrorMessage));
            else
            {
                var result = await _orderItemService.GetByOrderCommand(_orderID).ExecuteAsync();
                CurrentOrderItems = result.Value;
                var errors = await Task.WhenAll(CurrentOrderItems.Select(i => _orderItemService.DeleteCommand(i.ID).GetErrorsAsync()));
                foreach (var error in errors.SelectMany(i => i))
                    results.Add(error);
            }
            return results;
        }
    }
}
