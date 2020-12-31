using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTest
{
    public class Order
    {
        public int OrderNumber { get; set; }

        public string PurchaseOrderNumber { get; set; }
        public string BuyerName { get; set; }
        public string BillingZipCode { get; set; }

        public float OrderAmount { get; set; }
    }
}
