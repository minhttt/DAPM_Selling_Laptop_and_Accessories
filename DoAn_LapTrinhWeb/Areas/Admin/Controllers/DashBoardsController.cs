using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Windows.Media;

namespace DoAn_LapTrinhWeb.Areas.Admin.Controllers
{
    public class DashBoardsController : BaseController
    {
        private readonly DbContext db = new DbContext();
        // GET: Admin/DashBoards

        public ActionResult Index()
        {
            ViewBag.Order = db.Orders.ToList();
            ViewBag.OrderDetail = db.Oder_Detail.ToList();
            ViewBag.ListOrderDetail = db.Oder_Detail.OrderByDescending(m => m.create_at).Take(3).ToList();
            ViewBag.ListOrder = db.Orders.Take(7).ToList();

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;


            // Thống kê tổng số laptop bán được trong tháng
            var laptopStats = db.Oder_Detail.Where(m => m.Product.type == 1 &&
                m.Order.oder_date.Month == currentMonth &&
                m.Order.oder_date.Year == currentYear &&
                m.Order.status == "3")
               .GroupBy(m => m.Product.Brand.brand_name)
               .Select(p => new
               {
                   BrandName = p.Key,
                   TotalSold = p.Sum(m => m.quantity),
                   Total = p.Sum(m => m.quantity * m.price),
               }).OrderByDescending(x => x.TotalSold).ToList();

            var totalLaptopSold = laptopStats.Sum(x => x.TotalSold);
            ViewBag.TotalLaptopSold = totalLaptopSold;
            // Tính tỷ lệ phần trăm bán được cho từng nhãn hàng
            var laptopPercentages = laptopStats.Select(x => new
            {
                x.BrandName,
                x.TotalSold,
                Percentage = totalLaptopSold > 0 ? (double)x.TotalSold / totalLaptopSold * 100 : 0
            }).ToList();
            ViewBag.LaptopStatsJson = JsonConvert.SerializeObject(laptopPercentages);


            var accessoryStats = db.Oder_Detail
               .Where(od => od.Product.type == 2
                   && od.Order.oder_date.Month == currentMonth
                   && od.Order.oder_date.Year == currentYear
                   && od.Order.status == "3")
               .GroupBy(od => od.Product.Brand.brand_name)
               .Select(g => new
               {
                   BrandName = g.Key,
                   TotalSold = g.Sum(od => od.quantity),
                   Revenue = g.Sum(od => od.quantity * od.price)
               })
               .OrderByDescending(x => x.TotalSold)
               .ToList();

            var accessoryPercentages = accessoryStats.Select(x => new
            {
                x.BrandName,
                x.TotalSold,
                Percentage = totalLaptopSold > 0 ? (double)x.TotalSold / totalLaptopSold * 100 : 0
            }).ToList();
            ViewBag.AccessoryStatsJson = JsonConvert.SerializeObject(accessoryPercentages);
            ViewBag.CurrentMonth = currentMonth;
            ViewBag.CurrentYear = currentYear;


            var totalOrderPriceByMonth = db.Orders.Where(m => m.status == "3").GroupBy(m => m.oder_date.Month).Select(z => new
            {
                Month = z.Key,
                TotalSoldMoney = z.Sum(od => od.total),
            }).ToList();


            var allMonths = Enumerable.Range(1, 12).Select(month => new
            {
                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                MonthNumber = month,
                TotalSoldMoney = totalOrderPriceByMonth.FirstOrDefault(x => x.Month == month)?.TotalSoldMoney ?? 0
            }).ToList();



            var chartData = new
            {
                labels = allMonths.Select(x => x.Month).ToArray(), // Tháng 

                datasets = new[]
                {
                   new
                   {
                    label = "Doanh thu theo tháng",
                    data = allMonths.Select(x => x.TotalSoldMoney).ToArray(),
                   }
                }
            };
            ViewBag.TotalMoneyLineJson = JsonConvert.SerializeObject(chartData);
            return View();




        }
    }
}