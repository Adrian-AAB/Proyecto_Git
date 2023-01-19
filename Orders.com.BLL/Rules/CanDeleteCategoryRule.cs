﻿using Orders.com.BLL.DataProxy;
using Peasy;
using System.Linq;
using System.Threading.Tasks;

namespace Orders.com.BLL.Rules
{
    public class CanDeleteCategoryRule : RuleBase
    {
        private long _categoryID;
        private IProductDataProxy _productsDataProxy;

        public CanDeleteCategoryRule(long categoryID, IProductDataProxy productsDataProxy) 
        {
            _categoryID = categoryID;
            _productsDataProxy = productsDataProxy;
        }

        protected override void OnValidate()
        {
            var products = _productsDataProxy.GetByCategory(_categoryID);
            if (products.Any())
            {
                Invalidate("This category is associated with one or more products and cannot be deleted.");
            }
        }

        protected override async Task OnValidateAsync()
        {
            var products = await _productsDataProxy.GetByCategoryAsync(_categoryID);
            if (products.Any())
            {
                Invalidate("This category is associated with one or more products and cannot be deleted.");
            }
        }
    }
}
