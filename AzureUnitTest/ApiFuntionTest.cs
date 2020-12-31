using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace AzureUnitTest
{
    [TestClass]
    public class ApiFuntionTest
    {
        public const string HostUrl = "http://localhost:7071/api";

        [TestMethod]
        public void PostOrderTest()
        {
            string url = HostUrl + "/PostOrder";

            string tpl = @"{
	            ""BuyerName"":""Test User {0}"",
                ""PurchaseOrderNumber"":""Test PO {0}"",
	            ""BillingZipCode"":""5001"",
	            ""OrderAmount"": 59.99
            }";


            var postOrder = string.Format(tpl, Guid.NewGuid().ToString()); 
            string status = "";
            PostResponse(url, postOrder, out status);
            //content created
            Assert.AreEqual(status, "201");
            PostResponse(url, postOrder, out status);
            //content 
            Assert.AreEqual(status, "204");


            postOrder = string.Format(@"{
                ""BuyerName"":""Test User {0}"",
                ""PurchaseOrderNumber"":""Test PO {0}"",
	            ""OrderAmount"": 59.99
            }", Guid.NewGuid().ToString());

            PostResponse(url, postOrder, out status);
            //Missing
            Assert.AreEqual(status, "400");
        }

        [TestMethod]
        public void GetOrdersTest()
        {
            string url = HostUrl + "/GetOrders";
            string status = "";
            string postOrder = string.Empty;
            var result = PostResponse(url, postOrder, out status);
            dynamic data = JsonConvert.DeserializeObject(result);
            //get orders
            Assert.IsTrue(data.Length >0);
            Assert.IsNotNull(data[0]);
            Assert.IsNotNull(data[0].PurchaseOrderNumber);

            Dictionary<string, string> queryString = new Dictionary<string, string>();
            queryString["BuyerName"] = data[0].BuyerName;
            var result2 = PostResponse(url, queryString, out status);
            dynamic data2 = JsonConvert.DeserializeObject(result2);
            //"BuyerName Filter"
            Assert.IsTrue(data2.Length == 1);
            Assert.IsNotNull(data2[0].BuyerName);
            Assert.AreEqual(data2[0].BuyerName,data[0].BuyerName);

            queryString.Clear();
            queryString["PurchaseOrderNumber"] = data[0].PurchaseOrderNumber;
            var result3 = PostResponse(url, queryString, out status);
            dynamic data3 = JsonConvert.DeserializeObject(result3);
            //"PurchaseOrderNumber Filter"
            Assert.IsTrue(data3.Length == 1);
            Assert.IsNotNull(data3[0].PurchaseOrderNumber);
            Assert.AreEqual(data3[0].PurchaseOrderNumber, data[0].PurchaseOrderNumber);


            queryString.Clear();
            queryString["BillingZipCode"] = data[0].BillingZipCode;
            var result4 = PostResponse(url, queryString, out status);
            dynamic data4 = JsonConvert.DeserializeObject(result4);
            //"BillingZipCode Filter"
            Assert.IsTrue(data3.Length == 1);
            Assert.IsNotNull(data3[0].BillingZipCode);
            Assert.AreEqual(data3[0].PurchaseOrderNumber, data[0].BillingZipCode);

        }

        public string PostResponse(string url, string postData, out string statusCode)
        {
            string result = string.Empty;
            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            httpContent.Headers.ContentType.CharSet = "utf-8";
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;
                statusCode = response.StatusCode.ToString();
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            return result;
        }

        public string PostResponse(string url, Dictionary<string,string> queryStrings, out string statusCode)
        {
            var postData = new FormUrlEncodedContent(queryStrings).ToString();
            return PostResponse(url, postData, out statusCode);
        }
    }
}
