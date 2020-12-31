using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;

namespace AzureTest
{
    public class OrderService
    {
        private static OrderService _instance = new OrderService();
        private DbHelper _db;

        public static OrderService Instance
        {
            get
            {
                return _instance;
            }
        }

        public OrderService()
        {
            _db = new DbHelper(Environment.GetEnvironmentVariable("ConnectionString"));
        }

        public OrderService(string connectionString)
        {
            _db = new DbHelper(connectionString);
        }

        protected DbHelper GetDbHelp()
        {
           return _db;
        }

        public bool AddOrder(Order order)
        {
            var db = GetDbHelp();
            try
            {
                int count = db.ExecuteNonQuery("insert into [orders](BuyerName,PurchaseOrderNumber,BillingZipCode,OrderAmount) values(@BuyerName,@PurchaseOrderNumber,@BillingZipCode,@OrderAmount)",
                    db.CreateInParameter("BuyerName", SqlDbType.VarChar, order.BuyerName),
                    db.CreateInParameter("BillingZipCode", SqlDbType.VarChar, order.BillingZipCode),
                    db.CreateInParameter("PurchaseOrderNumber", SqlDbType.VarChar, order.PurchaseOrderNumber),
                    db.CreateInParameter("OrderAmount", SqlDbType.Float, order.OrderAmount)
                );
                return count == 1;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public List<Order> GetOrders(string? buyerName,string? purchaseOrderNumber,string? billingZipCode,float? orderAmount)
        {
            var query = new StringBuilder();
            var db = GetDbHelp();
            List<SqlParameter> parameters = new List<SqlParameter>();
            query.Append("select OrderNumber,PurchaseOrderNumber,BuyerName,BillingZipCode,OrderAmount from [Orders]");
            
            if(!string.IsNullOrEmpty(buyerName))
            {
                parameters.Add(db.CreateInParameter("BuyerName", SqlDbType.VarChar, buyerName));
            }
            if (purchaseOrderNumber != null)
            {
                parameters.Add(db.CreateInParameter("PurchaseOrderNumber", SqlDbType.VarChar, purchaseOrderNumber));
            }
            if (!string.IsNullOrEmpty(billingZipCode))
            {
                parameters.Add(db.CreateInParameter("BillingZipCode", SqlDbType.VarChar, billingZipCode));
            }
            if (orderAmount != null && orderAmount>0)
            {
                parameters.Add(db.CreateInParameter("OrderAmount", SqlDbType.Float, orderAmount));
            }


            for (int i=0;i<parameters.Count;i++)
            {
                if(i==0)
                {
                    query.AppendFormat(" where {0}=@{0}", parameters[i].ParameterName);
                }
                else
                {
                    query.AppendFormat(" and {0}=@{0}", parameters[i].ParameterName);
                }
            }
            var orders = new List<Order>();
            var dataset = db.ExecuteDataSet(query.ToString(), parameters.ToArray());
            if(dataset.Tables[0].Rows.Count > 0)
            {
                foreach(DataRow row in dataset.Tables[0].Rows)
                {
                    var order = new Order();
                    order.OrderNumber = int.Parse(row["OrderNumber"].ToString());
                    order.BuyerName = row["BuyerName"].ToString();
                    order.BillingZipCode = row["BillingZipCode"].ToString();
                    order.PurchaseOrderNumber = row["PurchaseOrderNumber"].ToString();
                    order.OrderAmount = float.Parse(row["OrderAmount"].ToString());
                    orders.Add(order);
                }
            }
            return orders;
        }
    }
}
