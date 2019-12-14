using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompileError.Model.Model;

using CompilerError.Models;
using Rotativa;
using AutoMapper;
using CompileError.Manager.Manager;
using CompileError.Models;

namespace CompilerError.Controllers
{

    public class SalesController : Controller
    {
        SalesViewModel salesViewModel = new SalesViewModel();
        CustomerManager _customerManager = new CustomerManager();
        CategoryManager _categoryManager = new CategoryManager();
        ProductManager _productManager = new ProductManager();
        PurchaseDetailManager _purchaseDetailManager = new PurchaseDetailManager();
        SalesDetailManager _salesDetailManager = new SalesDetailManager();
        SaleManager _saleManager = new SaleManager();
        SalesDetail salesDetail = new SalesDetail();
        PurchasedProductManager _purchasedProductManager = new PurchasedProductManager();

        PurchasedProduct purchasedProduct = new PurchasedProduct();
        Customer customer = new Customer();

        [HttpGet]
        public ActionResult Sales()
        {

            var customers = _customerManager.GetAll();
            //  var customer = from c in customers select (new { c.Id, c.Name });
            salesViewModel.CustomersSelectListItem = customers

                .Select(c => new SelectListItem()
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();


            ViewBag.Customer = salesViewModel.CustomersSelectListItem;

            var category = _categoryManager.GetAll();
            salesViewModel.CategorySelectListItem = category

                .Select(c => new SelectListItem()
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();


            ViewBag.Category = salesViewModel.CategorySelectListItem;
            return View(salesViewModel);

        }
        public ActionResult PrintViewToPdf()
        {
            var report = new ActionAsPdf("Search");
            return report;
        }

       
        [HttpGet]
        public ActionResult Search()
        {
            SalesViewModel salesViewModel = new SalesViewModel();
            salesViewModel.Sales = _saleManager.GetAll().ToList();



            return View(salesViewModel);
        }
        [HttpPost]
        public ActionResult Search(string option, string search)
        {

            List<Sale> sales = _saleManager.GetAll().ToList();


            if (option == "Code")
            {
                if (!string.IsNullOrEmpty(search))

                    sales = sales
                    .Where(c => c.Code.ToLower().Contains(search.ToLower())).ToList();

            }


            salesViewModel.Sales = sales;

            return View(salesViewModel);
        }
        [HttpGet]
        public ActionResult SaleDetail(int id)
        {
            //SalesViewModel salesViewModel = Mapper.Map<SalesViewModel>(_salesDetailManager.Search(id));
            salesViewModel.SalesDetails = _salesDetailManager.GetAll().Where(c => c.SaleId == id).ToList();
            return View(salesViewModel);
        }



        public JsonResult GetLoyalityPointByCustomer(int customerid)
        {
            var customerList = _customerManager.GetAll().Where(c => c.Id == customerid).ToList();
            var customers = from s in customerList select (s.LoyalityPoint);
            return Json(customers, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetProductByCategory(int categoryId)
        {
            var productList = _productManager.GetAll().Where(c => c.CategoryId == categoryId).ToList();
            var products = from s in productList select (new { s.Id, s.Name });
            return Json(products, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetQuantityByProduct(int productcode)
        {
            //var productList = _purchasedProductManager.GetAll().Where(c => c.ProductId == productcode).ToList();
            ////var productList = _purchaseDetailManager.GetAll().Where(c => c.Code == productcode).ToList();
            //var products = from s in productList select (new { s.Quantity, s.Mrp });
            //return Json(products, JsonRequestBehavior.AllowGet);

            PurchaseModelView purchaseModelView = new PurchaseModelView();

            var allPurchased = _purchasedProductManager.GetAll().Where(c => c.ProductId == productcode).ToList();
            //var productCode = _productManager.GetById(productId).Code;
            var allSold = _salesDetailManager.GetAll().Where(c => c.ProductId == productcode).ToList();
            var aq=0.0;
            foreach (var d in allPurchased)
            {
                aq += d.Quantity;
            }
            foreach (var d in allSold)
            {
                aq -= d.Quantity;
            }

            purchaseModelView.Quantity = aq;

            purchaseModelView.Mrp = _purchaseDetailManager.GetAll().FirstOrDefault(c => c.ProductId == productcode).Mrp;

            return Json(purchaseModelView, JsonRequestBehavior.AllowGet);

        }
        public JsonResult AddSales(int CustomerID, string Date, double Loyalitypoint, double Grandtotal, double Discount)
        {

            var sales = _saleManager.GetAll().ToList();
            string salecode = "";
            if (sales.Count() > 0)
            {
                var code = (from sa in sales orderby sa.Id descending select sa.Code).First();
                salecode = code.ToString();
                //salecode = code.ToString();
                string sub = salecode.Substring(5, 4);
                int c = Convert.ToInt32(sub);
                c++;
                string s = c.ToString("0000");
                salecode = "2019-" + s;
            }
            else
            {
                salecode = "2019-0001";


            }

            Sale sale = new Sale();
            sale.CustomerId = CustomerID;
            sale.Date = Date;


            sale.Code = salecode;

            _saleManager.Add(sale);

            double loyalityPointIncrease = (Loyalitypoint + (Grandtotal / 1000));
            double updateLoyalitypoint = (loyalityPointIncrease - Discount);
            List<Customer> customers = _customerManager.GetAll().Where(c => c.Id == CustomerID).ToList();
            foreach (var c in customers)
            {
                customer.Id = c.Id;
                customer.Name = c.Name;
                customer.Code = c.Code;
                customer.Contact = c.Contact;
                customer.Email = c.Email;
                customer.Address = c.Address;

            }
            customer.LoyalityPoint = updateLoyalitypoint;
            _customerManager.Update(customer);


            var productList = _saleManager.GetAll().Where(c => c.Code == salecode).ToList();
            var salesId = from s in productList select (s.Id);


            return Json(salesId, JsonRequestBehavior.AllowGet);


        }
        public JsonResult AddSalesDetails(int ProductCode, int Quantity, double MRP, double TotalMRP, int SalesID, double Aquantity)
        {

            salesDetail.ProductId = ProductCode;
            salesDetail.Quantity = Quantity;
            salesDetail.MRP = MRP;
            salesDetail.TotalMRP = TotalMRP;
            salesDetail.SaleId = SalesID;

            _salesDetailManager.Add(salesDetail);

            double qu = Convert.ToDouble(Quantity);
            double aq = Aquantity;

            double quantity = aq - qu;
            // purchasedProduct.ProductId = ProductCode;

            List<PurchasedProduct> purchasedProducts = _purchasedProductManager.GetAll().Where(c => c.ProductId == ProductCode).ToList();
            foreach (var c in purchasedProducts)
            {
                purchasedProduct.Id = c.Id;
                purchasedProduct.PurchaseId = c.PurchaseId;
                purchasedProduct.ProductId = c.ProductId;
                purchasedProduct.ManufactureDate = c.ManufactureDate;
                purchasedProduct.ExpireDate = c.ExpireDate;
                purchasedProduct.Mrp = c.Mrp;
                purchasedProduct.Remarks = c.Remarks;
                purchasedProduct.UnitPrice = c.UnitPrice;
            }
            purchasedProduct.Quantity = quantity;





            // _customerManager.Update(customer);
            _purchasedProductManager.Update(purchasedProduct);




            string mess = "success";

            return Json(mess, JsonRequestBehavior.AllowGet);

        }
        public double CalculateLOyalityPoint()
        {
            double loyality = 0;
            return loyality;
        }


    }
}