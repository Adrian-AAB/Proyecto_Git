﻿using Orders.com.BLL.Domain;
using Orders.com.BLL.Extensions;
using Peasy;

namespace Orders.com.BLL.Rules
{
    public class CanSubmitOrderItemRule : RuleBase
    {
        private OrderItem _orderItem;

        public CanSubmitOrderItemRule(OrderItem orderItem)
        {
            _orderItem = orderItem;
        }

        protected override void OnValidate()
        {
            if (!_orderItem.OrderStatus().CanSubmit)
            {
                Invalidate(string.Format("Order Item ID:{0} is in a {1} state and cannot be submitted", _orderItem .ID.ToString(), _orderItem .OrderStatus().Name));
            }
        }
    }
}
