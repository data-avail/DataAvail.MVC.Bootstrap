using DataAvail.MVC.Bootstrap.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAvail.MVC.Bootstrap.Controllers
{
    public class DataContextController<TDataContext, TDom> : Controller
        where TDataContext : IDataContext<TDom>
    {
        public DataContextController(TDataContext DataContext, Func<TDom, int?> GetParentId = null)
        {
            _dataContext = DataContext;
            _getParentId = GetParentId;
        }

        private readonly TDataContext _dataContext;

        private readonly Func<TDom, int?> _getParentId;

        protected TDataContext DataContext { get { return _dataContext; } }

        [NonAction]
        public virtual int? GetParentId(TDom Dom)
        {
            return _getParentId(Dom);
        }

        public ActionResult Index()
        {            
            var indexVM = GetIndexVM();
            @ViewBag.PageViewModel = indexVM;
            return View(_dataContext.GetAll(null, null));
        }

        public ActionResult IndexJson()
        {
            try
            {
                System.Dynamic.ExpandoObject obj = null;

                if (Request.QueryString.Count != 0)
                {
                    obj = new System.Dynamic.ExpandoObject();
                    foreach (var key in Request.QueryString.AllKeys)
                    {
                        ((IDictionary<String, Object>)obj).Add(key, Request.QueryString[key]);
                    }
                }

                var items = DataContext.GetAll(null, obj);
                     
                return Json(items, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception Exception)
            {
                return Json(OnJsonException(Exception, "Index"));
            }

        }

        [NonAction]
        protected virtual string GetIndexVM()
        {
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
            return "IndexVM";
=======
            return "Ural/VM/indexVM";
>>>>>>> master
=======
            return "Ural/VM/indexVM";
>>>>>>> master
=======
            return "Ural/VM/indexVM";
>>>>>>> master
        }

        protected object SuccessJson
        { 
            get { return new {success = true}; }
        }

        [HttpPost]
        public ActionResult GetNewJson(int? id)
        {
            return Json(_dataContext.GetNew(id));
        }

        public ActionResult Details(int id)
        {
            @ViewBag.PageViewModel = GetDetailsVM();
            return View(_dataContext.GetOne(id));
        }

        protected virtual string GetDetailsVM()
        {
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
            return "ItemVM";
=======
            return "Ural/VM/itemVM";
>>>>>>> master
=======
            return "Ural/VM/itemVM";
>>>>>>> master
=======
            return "Ural/VM/itemVM";
>>>>>>> master
        }

        public ActionResult Create(int? id)
        {
            @ViewBag.PageViewModel = "ItemVM";
            return View("Edit", _dataContext.GetNew(id));
        }

        [HttpPost]
        public ActionResult Create(TDom Item, FormCollection Values)
        {
            try
            {
                Item = GetDomItemForCreate(Item, Values);

                var id = _dataContext.Create(Item);

                return RedirectToAction("Details", new {id = id});
            }
            catch
            {
                _dataContext.InitializeCrumbs(Item, GetParentId(Item));
                @ViewBag.PageViewModel = "ItemVM";
                return View("Edit", Item);
            }
        }

        [HttpPost]
        public ActionResult CreateJson(TDom Item, FormCollection Values)
        {
            try
            {
                CheckDecimals(Item, Values);

                Item = GetDomItemForCreate(Item, Values);

                var id = _dataContext.Create(Item);

                return Json(_dataContext.GetOne(id));
            }
            catch(System.Exception Exception)
            {
                return Json(OnJsonException(Exception, "Create"));
            }

        }

        [NonAction]
        protected virtual object OnJsonException(Exception Exception, string Action)
        {
            var msg = Exception.InnerException != null ? Exception.InnerException.Message : Exception.Message;
            if (msg.Contains("DELETE") && msg.Contains("FK_"))
                msg = "Удаление невозможно, существуют связанные элементы";

            return new { error = msg };
        }

        [NonAction]
        protected virtual TDom GetDomItemForCreate(TDom Item, FormCollection Values)
        {
            return GetDomItem(Item, Values);
        }

        public ActionResult Edit(int id)
        {
            var order = _dataContext.GetOne(id);
            @ViewBag.PageViewModel = "ItemVM";

            return View(order);
        }

        [HttpPost]
        public ActionResult Edit(TDom Item, FormCollection Values)
        {
            try
            {
                Item = GetDomItemForEdit(Item, Values);

                var id = _dataContext.Update(Item);

                return RedirectToAction("Details", new { id = id });
            }
            catch
            {
                _dataContext.InitializeCrumbs(Item, GetParentId(Item));
                @ViewBag.PageViewModel = "ItemVM";
                return View(Item);
            }
        }

        [NonAction]
        private static void CheckDecimals(TDom Item, FormCollection Values)
        {
            foreach (var prop in Item.GetType().GetProperties().Where(p => p.PropertyType == typeof(double) || p.PropertyType == typeof(double?)))
            {
                var val = Values.GetValue(prop.Name);
                if (val != null && !string.IsNullOrEmpty(val.AttemptedValue))
                {
                    var d = double.Parse(val.AttemptedValue, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                    prop.SetValue(Item, d);
                }
            }
        }

        [HttpPost]
        public ActionResult EditJson(TDom Item, FormCollection Values)
        {
            try
            {
                CheckDecimals(Item, Values);

                Item = GetDomItemForEdit(Item, Values);

                var id = _dataContext.Update(Item);

                return Json(_dataContext.GetOne(id));
            }
            catch(System.Exception Exception)
            {
                return Json(OnJsonException(Exception, "Edit"));
            }
        }


        [NonAction]
        protected virtual TDom GetDomItemForEdit(TDom Item, FormCollection Values)
        {
            return GetDomItem(Item, Values);
        }

        [NonAction]
        protected virtual TDom GetDomItem(TDom Item, FormCollection Values)
        {
            return Item;
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                _dataContext.Delete(id);
                return Json(new { success = true });
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult DeleteJson(int id)
        {
            try
            {
                _dataContext.Delete(id);
                return Json(new { success = true });
            }
            catch(System.Exception Exception)
            {
                return Json(OnJsonException(Exception, "Delete"));
            }
        }

    }
}
