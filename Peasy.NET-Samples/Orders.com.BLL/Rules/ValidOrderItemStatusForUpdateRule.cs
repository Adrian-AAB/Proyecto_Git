﻿using Orders.com.BLL.Domain;
using Orders.com.BLL.Extensions;
using Peasy;

namespace Orders.com.BLL.Rules
{
    public class ValidOrderItemStatusForUpdateRule : RuleBase
    {
        private OrderItem _item;

        public ValidOrderItemStatusForUpdateRule(OrderItem item)
        {
            _item = item;
        }

        protected override void OnValidate()
        {
            if (_item.OrderStatus().IsBackordered)
            {
                Invalidate("Backordered items cannot be changed");
            }
            else if (_item.OrderStatus().IsShipped)
            {
                Invalidate("Shipped items cannot be changed");
            }
        }
    }
}
