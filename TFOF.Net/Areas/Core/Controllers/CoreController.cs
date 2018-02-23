namespace TFOF.Areas.Core.Controllers
{
    using TFOF.Areas.Core.Attributes;
    using TFOF.Areas.Core.Forms;
    using TFOF.Areas.Core.Services;
    using Microsoft.AspNet.Identity;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;

    [SiteAuthorize]
    public class CoreController : Controller
    {
        public CoreController()
        {

        }

        public static Dictionary<string, string> Layouts = new Dictionary<string, string>()
        {
            { "Modal" , "~/Views/Shared/_ModalLayout.cshtml" },
            { "ModalLarge" , "~/Views/Shared/_ModalLargeLayout.cshtml" }
        };
        
        public ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home", new { area = "Home" });
        }

        public string UserId
        {
            get { return User.Identity.GetUserId(); }
        }


        public List<String> GetRoles(IPrincipal user)
        {
            Attribute attr = this.GetType().GetCustomAttribute(typeof(SiteAuthorize));
            List<string> roles = new List<string>();
            if (attr != null)
            {
                foreach (string role in ((SiteAuthorize)attr).Roles.Split(',').ToList())
                {   
                    if(!string.IsNullOrWhiteSpace(role)) roles.Add((user.IsInRole(role) ? "*" : "") + role);
                }
            }
            return roles;
        }
    }

    /// <summary>
    /// Universal Controller
    /// </summary>
    /// <typeparam name="T">A Model</typeparam>
    [SiteAuthorize]
    public class CoreController<T> : Controller where T : class
    {
        public CoreController()
        {
        }

        /// <summary>
        /// DBSet to use by the controller.
        /// </summary>
        //public DbSet DBSet { get; set; }
        /// <summary>
        /// The DB context for create, updates.
        /// </summary>
        public BaseModelContext<T> DB { get; set; } = new BaseModelContext<T>();
        //public BaseModelContext<T> DBT { get; set; }
        /// <summary>
        /// The Form to use to render the editor.
        /// </summary>
        public Form Form { get; set; }
        /// <summary>
        /// The primary key field of the associated model this controller uses.
        /// </summary>
        public string PrimaryKey { get; set; } = "Id";
        /// <summary>
        /// The foreign key field that the model is related to.
        /// </summary>
        public string ForeignKey { get; set; }
        /// <summary>
        /// Can the user delete the model record?
        /// </summary>
        public bool CanDelete { get; set; } = false;
        /// <summary>
        /// Can the user create records for this model?
        /// </summary>
        public bool CanCreate { get; set; } = true;
        /// <summary>
        /// A list of search forms to be used on any page.
        /// By default Index.cshtml uses 'SearchForm'. 
        /// </summary>
        public List<SearchForm> SearchForms { get; set; } = new List<SearchForm>();
        /// <summary>
        /// The title for the index page or any page that would like to use it.
        /// </summary>
        public string TitlePlural { get; set; }

        /// <summary>
        /// The ReturnUrl directs the user after a deletion or by clicking on the last clickable breadcrumb.
        /// </summary>
        public ReturnTo ReturnTo { get; set; }

        /// <summary>
        /// A navigation item property for inserting breadcrumbs in between breadcrumbs
        /// </summary>
        private NavigationItem BreadcrumbItem { get; set; }


        /// <summary>
        /// A dictionary of layout templates. The key is passed in the QueryString.
        /// Example: ?Layout=Modal
        /// </summary>
        public static Dictionary<string, string> Layouts = new Dictionary<string, string>()
        {
            { "Modal" , "~/Views/Shared/_ModalLayout.cshtml" },
            { "ModalLarge" , "~/Views/Shared/_ModalLargeLayout.cshtml" },
            { "ModalSmall" , "~/Views/Shared/_ModalSmallLayout.cshtml" }
        };



        private void Layout(HttpRequestBase request)
        {
            if (request.QueryString.Get("Layout") != null)
            {
                ViewBag.Layout = Layouts[request.QueryString.Get("Layout")];
            }
        }

        public List<String> GetRoles(IPrincipal user, string method = null)
        {
            MethodInfo[] methodInfos = this.GetType().GetMethods();
            //Attribute methodAttr = this.GetType().GetMethod(method,).GetCustomAttribute(typeof(SiteAuthorize));
            Attribute classAttr = this.GetType().GetCustomAttribute(typeof(SiteAuthorize));
            List<string> roles = new List<string>();
            
            if (classAttr != null)
            {
                foreach(string role in ((SiteAuthorize)classAttr).Roles.Split(',').ToList())
                {
                    if (!string.IsNullOrWhiteSpace(role) && !roles.Contains(role)) roles.Add((user.IsInRole(role) ? "*" : "") + role);
                }
            }

            if (methodInfos != null)
            {
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    if (methodInfo.Name.Equals(method))
                    {
                        Attribute methodAttr = methodInfo.GetCustomAttribute(typeof(SiteAuthorize));
                        if (methodAttr != null)
                        {
                            foreach (string role in ((SiteAuthorize)methodAttr).Roles.Split(',').ToList())
                            {
                                if (!string.IsNullOrWhiteSpace(role) && !roles.Contains(role)) roles.Add((user.IsInRole(role) ? "*" : "") + role);
                            }
                        }
                    }
                }
            }


            return roles;
        }

        /// <summary>
        /// Checks to make sure that DB, DBSet, and Form are set.
        /// </summary>
        private void CheckContext(Boolean checkForm = true)
        {
            List<string> missing = new List<string>();

            if (DB == null)
            {
                missing.Add("DB");
            }

            //if (DBSet == null)
            //{
            //    missing.Add("DBSet");
            //}

            if (Form == null)
            {
                missing.Add("Form");
            }
            if (checkForm)
            {
                if (missing.Count > 0)
                {
                    throw new Exception(string.Join(" and ", missing.ToArray()) + (missing.Count > 1 ? " properties are " : " property is ") + "missing");
                }
            }
        }

        /// <summary>
        /// The controller name
        /// </summary>
        private string ControllerName
        {
            get
            {
                return this.GetType().Name.Replace("Controller", "");
            }
        }


        /// <summary>
        /// Gets one record from the DBSet trying with int id, long int, then string it.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetOne(string id)
        {
            T model = default(T);
            try
            {
                model = (T)DB.Models.Find(int.Parse(id));
            }
            catch
            {
                try
                {
                    model = (T)DB.Models.Find(Int64.Parse(id));
                }
                catch
                {
                    try
                    {
                        model = (T)DB.Models.Find(Guid.Parse(id));
                    }
                    catch
                    {
                        try
                        {
                            model = (T)DB.Models.Find(id);
                        }
                        catch
                        {
                            return model;
                        }
                    }
                }
            }

            return model;
        }

        private NavigationItem GetBreadcrumb(HttpRequestBase request)
        {
            NavigationItem navigationItem = new NavigationItem();
            string breadcrumb = null;
            if ((breadcrumb = Request.QueryString.Get("Breadcrumb")) != null)
            {
                if (breadcrumb.Split('|').Length > 1)
                {
                    navigationItem.Label = breadcrumb.Split('|')[0];
                    navigationItem.Link = breadcrumb.Split('|')[1];
                    return navigationItem;
                }
            }
            return null;
        }

        public Breadcrumbs GetBreadcrumbs(T model, UrlHelper url, HttpRequestBase request, string LastPage)
        {
            Breadcrumbs breadcrumbs = new Breadcrumbs(url);
            var indexPg = url.Action("Index", ControllerName);
            if (!string.IsNullOrWhiteSpace(indexPg))
            {
                breadcrumbs.AddCrumb(TitlePlural, url.Action("Index", ControllerName));
            }
            if ((BreadcrumbItem = GetBreadcrumb(Request)) != null)
            {
                breadcrumbs.Crumbs.Add(BreadcrumbItem);
            }
            if (ReturnTo != null)
            {
                ReturnTo.GetUrl(model, url);
                if (ReturnTo.Url != null)
                {
                    if (!string.IsNullOrWhiteSpace(ReturnTo.Id) && !string.IsNullOrWhiteSpace(ReturnTo.Text))
                    {
                        breadcrumbs.AddCrumb(!string.IsNullOrWhiteSpace(ReturnTo.Text) ? ReturnTo.Text : "---", ReturnTo.Url);
                    }
                }
            }
            
            if (LastPage != null)
            {
                breadcrumbs.AddCrumb(LastPage);
            }

            return breadcrumbs;

        }

        private void SetCreateDeleteLink(T model = null)
        {
            if(model == null)
            {
                //If can create provide a link
                if (CanCreate) ViewBag.LinkNew = Url.Action("Create", ControllerName);
                return;
            }
            string fk = "";

            ///List because of composite keys
            List<string> pk = new List<string>();
            if (!string.IsNullOrWhiteSpace(ForeignKey))
            {
                PropertyInfo fprop = model.GetType().GetProperty(ForeignKey);
                if (fprop != null)
                {
                    fk = (fprop.GetValue(model) != null ? fprop.GetValue(model).ToString() : "");
                }
            }

            PropertyInfo[] properties = model.GetType().GetProperties();
            foreach(PropertyInfo kprop in properties)
            {
                var attr = kprop.GetCustomAttribute(typeof(KeyAttribute));
                if (attr is KeyAttribute && kprop.GetValue(model) != null)
                {
                    pk.Add(kprop.GetValue(model).ToString());
                }

            }

            //If can create provide a link
            if (CanCreate) Form.NewLink = Url.Action("Create", ControllerName, new { id = fk }) ;

            //If can delete provide a link and delete messaging
            if (CanDelete && pk.Count == 1)
            {
                //Form.DeleteLink = Url.Action("Delete", ControllerName, new { id = pk[0] });
                Form.Delete = DeleteValidation(Url.Action("Delete", ControllerName, new { id = pk[0] }), pk[0]);
            }

        }

        public virtual BaseDeleteModel DeleteValidation(string url, string id)
        {
            return new BaseDeleteModel(url, id);
        }

     
        // GET: {area}/{controller}
        public virtual ActionResult Index()
        {

            Layout(Request);
            //Make all SearchForms available to the view.
            //Set the ApiUrl first.
            foreach (SearchForm searchForm in SearchForms)
            {
                searchForm.SetApiUrl(Url);
                //Set any Request Params
                foreach (string key in Request.Params.Keys)
                {
                    searchForm.SetSearchValue(key, new string[] { Request.Params.Get(key) });
                }
                ViewData[searchForm.Name] = searchForm;
            }
            
            //Set the title
            SetCreateDeleteLink();
            ViewBag.Title = TitlePlural;
            ViewBag.CurrentUserId = UserId;
            ViewBag.ViewRoles = GetRoles(User, "Index");
            return View();
        }

        // GET: {area}/{controller}/Details/{id}
        public virtual ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckContext(false);
            T model = GetOne(id);
            if (model == null)
            {
                return HttpNotFound();
            }


            //Make all SearchForms available to the view.
            //Set the ApiUrl first.
            foreach (SearchForm searchForm in SearchForms)
            {
                searchForm.SetApiUrl(Url);
                ViewData[searchForm.Name] = searchForm;
            }

            SetCreateDeleteLink(model);
            ViewData["Breadcrumbs"] = GetBreadcrumbs(model, Url, Request, "Details");
            if (Form != null)
            {
                ViewData["Form"] = Form.Init(model, User);
            }
            Layout(Request);
            ViewBag.CurrentUserId = UserId;
            ViewBag.ViewRoles = GetRoles(User, "Details");
            return View(model);
        }

        // GET: {area}/{controller}/{Id} [id = foreignKey]
        public virtual ActionResult Create(string id)
        {
            T model = (T)Activator.CreateInstance(typeof(T));
            
            //Set the foreignKey based on the ForeignKey Property
            if (!string.IsNullOrWhiteSpace(ForeignKey) && !string.IsNullOrWhiteSpace(id))
            {
                try
                {
                    model.GetType().GetProperty(ForeignKey).SetValue(model, int.Parse(id));
                }
                catch
                {
                    model.GetType().GetProperty(ForeignKey).SetValue(model, id);
                }
            }

            ViewData["Breadcrumbs"] = GetBreadcrumbs(model, Url, Request, "New");
            ViewData["Form"] = Form.Init(model, User);

            Layout(Request);
            ViewBag.CurrentUserId = UserId;
            ViewBag.ViewRoles = GetRoles(User, "Create");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: {area}/{controller}/
        public virtual ActionResult Create(T model)
        {
            CheckContext();

            if (ModelState.IsValid)
            {
                if (CreateModel(model) > 0)
                {
                    if (!TempData.ContainsKey("Success"))
                    {
                        TempData["Success"] = "Record Created!";
                    }
                    var _id = model.GetType().GetProperty(PrimaryKey).GetValue(model);

                    if (ReturnTo != null && ReturnTo.RedirectAfter != null && ReturnTo.RedirectAfter.Contains("Create"))
                    {
                        string url = ReturnTo.GetUrl(model, Url);
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            return Redirect(url);
                        }
                    }
                    return Redirect(Url.Action("Edit", new { Controller = ControllerName, Id = _id }));
                }
            }

            ViewData["Breadcrumbs"] = GetBreadcrumbs(model, Url, Request, "Create");
            ViewData["Form"] = Form.Init(model, User);

            Layout(Request);
            ViewBag.CurrentUserId = UserId;
            ViewBag.ViewRoles = GetRoles(User, "Create");
            return View(model);
        }

        // GET: {area}/{controller}/Edit/{id}
        public virtual ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckContext();
            T model = GetOne(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            SetCreateDeleteLink(model);
            ViewData["Breadcrumbs"] = GetBreadcrumbs(model, Url, Request, "Edit");
            ViewData["Form"] = Form.Init(model, User);
            Layout(Request);
            ViewBag.CurrentUserId = UserId;
            ViewBag.ViewRoles = GetRoles(User, "Edit");
            
            ViewData["AuditSearchForm"] = new AuditViewerSearchForm(Url, ObjectContext.GetObjectType(model.GetType()).Name, id);
            return View(model);
        }

        // POST: {area}/{controller}/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(T model)
        {
            CheckContext();
            var _id = model.GetType().GetProperty(PrimaryKey).GetValue(model);
            if (ModelState.IsValid)
            {
                if (UpdateModel(model) > 0)
                {
                    if (!TempData.ContainsKey("Success"))
                    {
                        TempData["Success"] = "Update Successful!";
                    }
                    if (ReturnTo != null && ReturnTo.RedirectAfter != null && ReturnTo.RedirectAfter.Contains("Edit"))
                    {
                        string url = ReturnTo.GetUrl(model, Url);
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            return Redirect(url);
                        }
                    }
                    return Redirect(Url.Action("Edit", new { Controller = ControllerName, Id = _id }));
                }
            }

            SetCreateDeleteLink(model);
            ViewData["Breadcrumbs"] = GetBreadcrumbs(model, Url, Request, "Edit");
            ViewData["Form"] = Form.Init(model, User);
            Layout(Request);
            ViewBag.CurrentUserId = UserId;
            ViewBag.ViewRoles = GetRoles(User, "Edit");
            return View(model);
        }

        // GET: {area}/{controller}/Delete/{id}
        public virtual ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T model = GetOne(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            BaseDeleteModel deleteModel = new BaseDeleteModel(Request.Url.ToString(), id);
            Layout(Request);
            return View(deleteModel);
        }

        // POST: {area}/{controller}/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(string id)
        {
            if (!CanDelete)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            T model = GetOne(id);
            string returnUrl = null;

            //Check where to return to.
            if (ReturnTo != null && ReturnTo.RedirectAfter != null && ReturnTo.RedirectAfter.Contains("Delete"))
            {
                ReturnTo.GetUrl(model, Url);
                if (!string.IsNullOrWhiteSpace(ReturnTo.Url))
                {
                    returnUrl = ReturnTo.Url;
                }
            }

            //Do the delete
            if (model != null)
            {
                DeleteModel(model);
                if (!TempData.ContainsKey("Success"))
                {
                    TempData["Success"] = "Deleted!";
                }
            }

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = Url.Action("Index", ControllerName);
                return RedirectToLocal(returnUrl);
            }
            return Redirect(returnUrl);
        }

        public virtual int CreateModel(T model)
        {
            DB.Models.Add(model);
            return DB.SaveChanges();
        }

        public virtual int UpdateModel(T model)
        {
            DB.Entry(model).State = EntityState.Modified;
            return DB.SaveChanges();
            
        }

        public virtual void DeleteModel(T model)
        {
            DB.Models.Remove(model);
            DB.SaveChanges();
        }
        
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DB.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home", new { area = "Home" });
        }

        public string UserId
        {
            get { return User.Identity.GetUserId(); }
        }

        public SearchForm GetSearchForm(string name)
        {
            foreach(SearchForm searchForm in SearchForms)
            {
                if(!string.IsNullOrWhiteSpace(searchForm.Name) && searchForm.Name.Equals(name))
                {
                    return searchForm;
                }
            }
            return null;
        }

        /// <summary>
        /// Works only if the images do not have extensions. TODO
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public ActionResult Image(string id)
        //{
        //    var partentViewContext = Request.RequestContext.RouteData.DataTokens;
        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Areas", partentViewContext["area"].ToString(), "Images", id); 
        //    return File(filepath, MimeMapping.GetMimeMapping(filepath));
        //}
    }

    /// <summary>
    /// ReturnTo redirects users to the parent model's page. It requires a controller as T, a foreign key field name from the child model.
    /// </summary>
    public class ReturnTo
    {
        public string ActionName { get; set; }
        public string Area { get; set; }
        public object Controller { get; set; }
        public string TextField { get; set; }
        public string ForeignKeyField { get; set; }
        public string Url { get; set; }
        public string Text { get; set; }
        public List<string> RedirectAfter { get; set; }
        public string Id { get; set; }

        /// <summary>
        /// Contruct a ReturnTo class to direct users to parent model. 
        /// </summary>
        /// <param name="controller">The controller instance to redirec to.</param>
        /// <param name="actionName">The action of the controller used.</param>
        /// <param name="foreignKeyField">The field in the child model where the parent id can be retrieved.</param>
        /// <param name="relatedModelDisplayField">The field to use to display as the link text</param>
        /// <param name="redirectAfter">A list of action result names that will be redirected after post. Options are Create and Edit. Delete by default re-directs.</param>
       
        public ReturnTo(object controller, string actionName, string foreignKeyField, string relatedModelDisplayField, string[] redirectAfter = null)
        {
            Controller = controller;
            ActionName = actionName;
            ForeignKeyField = foreignKeyField;
            TextField = relatedModelDisplayField;
            RedirectAfter = redirectAfter != null ? redirectAfter.ToList() : new List<string>();

        }
        public string GetUrl(object model, UrlHelper url)
        {
            dynamic _id  = null;
            //Update Url if null because it can be assigned manually through orverridden actionresults.
            if (model != null && ForeignKeyField != null && Url == null)
            {   
                PropertyInfo mp = model.GetType().GetProperty(ForeignKeyField);
                if (mp != null)
                {
                    _id = mp.GetValue(model, null);
                    if (_id != null && !string.IsNullOrWhiteSpace(_id.ToString()) && Controller != null)
                    {
                        var method = Controller.GetType().GetMethod("GetOne");
                        object[] parameters = new object[] { _id.ToString() };

                        if (method != null)
                        {
                            var returnToModel = method.Invoke(Controller, parameters);
                            if (returnToModel != null)
                            {
                                try
                                {
                                    Text = returnToModel.GetType().GetProperty(TextField).GetValue(returnToModel, null).ToString();
                                } catch {
                                    //Do nothing;
                                }
                            }
                        }
                    }
                }
            }

            try
            {
                _id = (_id != null ? _id.ToString() : "");
                Id = _id.ToString();
                if (Area != null)
                {
                    Url = url.Action(ActionName, Controller.GetType().Name.Replace("Controller", ""), new { id = _id, area = Area });
                }
                else
                {
                    Url = url.Action(ActionName, Controller.GetType().Name.Replace("Controller", ""), new { id = _id });

                }
            }
            catch
            {

            }
            return Url;
        }
    }
}
