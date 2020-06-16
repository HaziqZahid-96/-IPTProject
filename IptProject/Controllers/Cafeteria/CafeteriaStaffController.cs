using IptProject.Models.Cafeteria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace IptProject.Controllers.Cafeteria
{
    public class CafeteriaStaffController : Controller
    {
        public static List<FoodOrder> lstOrder = new List<FoodOrder>();
        // GET: CafeteriaStaff
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewOrders()
        {
            //List<FoodOrder> lstOrder = new List<FoodOrder>();
            lstOrder.Clear();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Shared.ServerConfig.GetBaseUrl());
                //HTTP GET
                var responseTask = client.GetAsync("cafeteriastaff/ViewOrders");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    //List<Dictionary<string, object>>
                    var readTask = result.Content.ReadAsAsync<FoodOrder[]>();
                    readTask.Wait();

                    var order = readTask.Result;

                    foreach (var item in order)
                    {
                        item.Datestr = item.OrderDate.ToString("dd-MM-yyyy");
                        item.Timestr = item.OrderTime.ToString("HH:MM");

                        lstOrder.Add(item);
                    }
                }
            }

            return View(lstOrder);
        }
        public ActionResult ViewDetails(int OrderId)
        {

            var orderdetails = lstOrder.Where(x => x.OrderID == OrderId).Select(x => x.OrderDetails).ToList();
            ViewBag.orderdetails = orderdetails[0];
            return PartialView();
        }
        public ActionResult EditOrderStatus(int OrderId)
        {

            FoodOrder obj = lstOrder.Where(x => x.OrderID == OrderId).FirstOrDefault();
            SelectList OrderStatus = Shared.Constants.getOrderStatus();
            ViewBag.OrderStatus = OrderStatus;
            return PartialView(obj);
        }
        [HttpPost]
        public ActionResult EditOrderStatus(FoodOrder obj)
        {
            //FoodItem objFooditem = new FoodItem();
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["OrderId"] = obj.OrderID;
            data["OrderStatus"] = obj.OrderStatus;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Shared.ServerConfig.GetBaseUrl());
                //HTTP GET
                var responseTask = client.PostAsJsonAsync("CafeteriaStaff/UpdateOrderStatus", data);

                responseTask.Wait();

                var result = responseTask.Result;
                if (result.StatusCode == HttpStatusCode.OK)
                {

                    return Content("Order Status Updated!");

                }
                else
                {
                    return Content("Err...There seems to be some error!");
                }
            }

        }




    }
}