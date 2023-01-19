﻿using Peasy;
using Peasy.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Orders.com.BLL.Domain
{
    public class Order : DomainBase
    {
        public long OrderID { get; set; }

        [PeasyForeignKey, PeasyRequired]
        public long CustomerID { get; set; }

        [Editable(false)]
        public DateTime OrderDate { get; set; }

        public override long ID
        {
            get { return OrderID; }
            set { OrderID = value; }
        }
    }
}
