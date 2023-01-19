﻿using Peasy;
using System.ComponentModel.DataAnnotations;

namespace Orders.com.BLL.Domain
{
    public class InventoryItem : DomainBase, IVersionContainer
    {
        public long InventoryItemID { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Quantity on Hand must be a non-negative value")]
        [Display(Name = "Quantity on Hand")]
        public decimal QuantityOnHand { get; set; }

        public long ProductID { get; set; }

        public override long ID
        {
            get { return InventoryItemID; }
            set { InventoryItemID = value; }
        }

        [Editable(false)]
        public string Version
        {
            get; set;
        }
    }
}
