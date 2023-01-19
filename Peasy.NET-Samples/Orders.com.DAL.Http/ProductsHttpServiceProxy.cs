﻿using Orders.com.BLL.DataProxy;
using Orders.com.BLL.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orders.com.DAL.Http
{
    public class ProductsHttpServiceProxy : OrdersDotComHttpProxyBase<Product, long>, IProductDataProxy
    {
        public ProductsHttpServiceProxy(string baseAddress) :base(baseAddress) { }

        protected override string RequestUri
        {
            get { return $"{BaseAddress}/products"; }
        }

        public IEnumerable<Product> GetByCategory(long categoryID)
        {
            return base.GET<IEnumerable<Product>>($"{RequestUri}?categoryid={categoryID}");
        }

        public Task<IEnumerable<Product>> GetByCategoryAsync(long categoryID)
        {
            return base.GETAsync<IEnumerable<Product>>($"{RequestUri}?categoryid={categoryID}");
        }
    }
}
