using IptProject.Models.Cafeteria;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace IptProject.Controllers
{
    public class CafeteriaController : Controller
    {
        public static List<FoodOrder> lstOrder = new List<FoodOrder>();
        // GET: Cafeteria
        public ActionResult GetProduct()
        {
            List<FoodItem> lstFoodItems = new List<FoodItem>();
            //FoodItem obj1 = new FoodItem(1, "Tikka", "avc", "Available", 200);
            //FoodItem obj2 = new FoodItem(2, "Pizza", "avc", "Available", 100);
            //lstFoodItems.Add(obj1);
            //lstFoodItems.Add(obj2);


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44380/api/");
                //HTTP GET
                var responseTask = client.GetAsync("cafeteria/getproduct");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<FoodItem[]>();
                    readTask.Wait();

                    var fooditems = readTask.Result;

                    foreach (var item in fooditems)
                    {
                        lstFoodItems.Add(item);
                    }
                }
            }

            return View();
        }
        public ActionResult fetchImage()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44380/api/");
                //HTTP GET
                var responseTask = client.GetAsync("testCafeteria/GetProductWithImage");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<Dictionary<string, object>>();
                    readTask.Wait();

                    var item = readTask.Result;
                    var base64string = item["base64string"];
                    var contents = Convert.FromBase64String(base64string.ToString());

                    MemoryStream ms = new MemoryStream(contents);
                    Image returnImage = Image.FromStream(ms);

                    //foreach (var item in fooditems)
                    //{
                    //    lstFoodItems.Add(item);
                    //}
                    ViewBag.Image = returnImage;
                }
            }
            return View();
        }
        public ActionResult GetImage()
        {
            List<FoodItem> lstFoodItems = new List<FoodItem>();
            //FoodItem obj1 = new FoodItem(1, "Tikka", "avc", "Available", 200);
            //FoodItem obj2 = new FoodItem(2, "Pizza", "avc", "Available", 100);
            //lstFoodItems.Add(obj1);
            //lstFoodItems.Add(obj2);


          
            return View();
        }
        public ActionResult ViewOrder()
        {
            List<FoodItem> lstFoodItems = new List<FoodItem>();
            lstOrder.Clear();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Shared.ServerConfig.GetBaseUrl());
                //HTTP GET
                int StudentId = 1;   //get from session

                var responseTask = client.GetAsync("cafeteria/GetOrdersbyStudentId?id=" + StudentId);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<FoodOrder[]>();
                    readTask.Wait();

                    var orders = readTask.Result;

                    foreach (var item in orders)
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





    }
}

