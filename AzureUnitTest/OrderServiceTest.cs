using AzureTest;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;

namespace AzureUnitTest
{
    [TestClass]
    public class OrderServiceTest
    {

        [TestMethod]
        public void OderTest()
        {
            Order order = new Order();
            Guid guid = Guid.NewGuid();
            
            order.PurchaseOrderNumber = "Test PO " + guid.ToString();
            order.BuyerName = "Test User " + guid.ToString();
            order.BillingZipCode = "4001";
            order.OrderAmount = 999.99f;

            OrderService service = new OrderService(ConfigurationManager.AppSettings["ConnectionString"]);

            var result = service.AddOrder(order);
            Assert.IsTrue(result);//successful insert

            //order.BuyerName = "Test User " + Guid.NewGuid();
            //order.BillingZipCode = "5001";
            //order.OrderAmount = 100.99f;
            //result = service.AddOrder(order);
            //Assert.IsFalse(result);//duplicate insert

            order.BillingZipCode = null;
            result = service.AddOrder(order);
            Assert.IsFalse(result);//without BillingZipCode

            //retrieve exit order
            var exitOrders = service.GetOrders(null, "Test PO " + guid.ToString(), null, null);
            Assert.IsNotNull(exitOrders);
            Assert.AreEqual(exitOrders[0].PurchaseOrderNumber,"Test PO " + guid.ToString());
        }

    }
}
