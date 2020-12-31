using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AzureTest
{
    public static class GetOrders
    {
        [FunctionName("GetOrders")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var request = new Order();
            request.BuyerName = req.Query["buyerName"];
            request.PurchaseOrderNumber = req.Query["purchaseOrderNumber"];
            request.BillingZipCode = req.Query["billingZipCode"];
            float orderAmount = 0;
            if(float.TryParse(req.Query["orderAmount"].ToString(), out orderAmount))
            {
                request.OrderAmount = orderAmount;
            }
            
            var orders = OrderService.Instance.GetOrders(request.BuyerName, request.PurchaseOrderNumber, request.BillingZipCode, request.OrderAmount);
            return new JsonResult(orders.ToArray());
         }
    }
}
