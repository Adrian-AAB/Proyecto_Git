﻿using Orders.com.BLL.Services;
using Peasy;
using System.Linq;
using System.Threading.Tasks;

namespace Orders.com.BLL.Rules
{
    public class CanDeleteCustomerRule : RuleBase
    {
        private long _customerID;
        private IOrderService _orderService;

        public CanDeleteCustomerRule(long customerID, IOrderService orderService)
        {
            _customerID = customerID;
            _orderService = orderService;
        }

        protected override void OnValidate()
        {
            var orders = _orderService.GetByCustomerCommand(_customerID).Execute().Value;
            if (orders.Any())
            {
                Invalidate("This customer is associated with one or more orders and cannot be deleted.");
            }
        }

        protected override async Task OnValidateAsync()
        {
            var orders = await _orderService.GetByCustomerCommand(_customerID).ExecuteAsync();
            if (orders.Value.Any())
            {
                Invalidate("This customer is associated with one or more orders and cannot be deleted.");
            }
        }
    }
}
