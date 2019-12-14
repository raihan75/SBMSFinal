﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CompileError.Model.Model;
using CompilerError.Models;

using AutoMapper;
using CompileError.Manager.Manager;

namespace CompilerError.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        CategoryManager _categoryManager = new CategoryManager();

        [HttpGet]
        public ActionResult Add()
        {
            CategoryViewModel categoryViewModel = new CategoryViewModel();
            categoryViewModel.Categories = _categoryManager.GetAll();
            return View(categoryViewModel);
        }
        [HttpPost]
        public ActionResult Add(CategoryViewModel categoryViewModel)
        {

            string message = "";
            if (ModelState.IsValid)
            {

                Category category = Mapper.Map<Category>(categoryViewModel);

                if (_categoryManager.Add(category))
                {
                    message = "save";

                }
                else
                {
                    message = "not saved";
                }

            }

            categoryViewModel.Categories = _categoryManager.GetAll();
            ViewBag.Message = message;
            return View(categoryViewModel);
        }

        [HttpGet]
        public ActionResult Update(int id)
        {
            CategoryViewModel categoryViewModel = Mapper.Map<CategoryViewModel>(_categoryManager.Search(id));
            categoryViewModel.Categories = _categoryManager.GetAll();
            return View(categoryViewModel);

        }
        [HttpPost]
        public ActionResult Update(CategoryViewModel categoryViewModel)
        {
            Category category = Mapper.Map<Category>(categoryViewModel);
            _categoryManager.Update(category);
            categoryViewModel.Categories = _categoryManager.GetAll();
            return View(categoryViewModel);
            // return RedirectToAction("Add");

        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            _categoryManager.Delete(id);
            return RedirectToAction("Add");

        }

        [HttpGet]
        public ActionResult Search()
        {
            CategoryViewModel categoryViewModel = new CategoryViewModel();
            categoryViewModel.Categories = _categoryManager.GetAll();
            return View(categoryViewModel);
        }

        [HttpPost]
        public ActionResult Search(string option, string search)
        {
            CategoryViewModel categoryViewModel = new CategoryViewModel();

            var categories = _categoryManager.GetAll();


            if (option == "Name")
            {
                if(!string.IsNullOrEmpty(search))

                    categories = categories
                    .Where(c => c.Name.ToLower().Contains(search.ToLower())).ToList();
  
            }
            else if (option == "Code")
            {
                if (!string.IsNullOrEmpty(search))

                    categories = categories
                    .Where(c => c.Code.ToLower().Contains(search.ToLower())).ToList();
            }


            categoryViewModel.Categories = categories;

            return View(categoryViewModel);
        }

        public JsonResult GetCodeUnique(string CategoryCode)
        {
            bool isHas = false;

            var categoryCode = _categoryManager.GetAll().Where(c => c.Code == CategoryCode);
            if (categoryCode.Count() > 0)
            {
                isHas = true;
            }

            return Json(isHas, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNameUnique(string CategoryName)
        {

            bool isHas = false;

            var categoryName = _categoryManager.GetAll().Where(c => c.Name == CategoryName);
            if (categoryName.Count() > 0)
            {
                isHas = true;
            }

            return Json(isHas, JsonRequestBehavior.AllowGet);
        }
    }
}