using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureTest
{
    public static class PostOrder
    {
        [FunctionName("PostOrder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Order order = JsonConvert.DeserializeObject<Order>(requestBody);
            var result = new ObjectResult(string.Empty);
            if (order == null || string.IsNullOrEmpty(order.PurchaseOrderNumber) || string.IsNullOrEmpty(order.BuyerName) || string.IsNullOrEmpty(order.BillingZipCode) || string.IsNullOrEmpty(order.OrderAmount.ToString()))
            {
                result.StatusCode = 400;
                return result;
            }

            var existOrder = OrderService.Instance.GetOrders(null, order.PurchaseOrderNumber, null, null);
            if (existOrder.Count > 0)
            {
                result.StatusCode = 204;
                return result;
            }

            var succuss = OrderService.Instance.AddOrder(order);
            if (succuss)
            {
                result.StatusCode = 201;
                return result;
            }

            return new OkResult();
        }
    }
}
