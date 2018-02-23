namespace TFOF.Areas.Core.Controllers.API
{
    using System.Web.Http;
    using System.Web.Http.OData.Query;
    using Microsoft.AspNet.Identity;
    using Models;
    using System.Data.Entity.Infrastructure;
    using System.Web.Http.Description;
    using Newtonsoft.Json.Linq;
    using Microsoft.Data.OData.Query.SemanticAst;
    using System.Collections.Generic;
    using Microsoft.Data.OData.Query;



    public class ODataFilter
    {
        public string Left { get; set; }
        public string Operator { get; set; }
        public string Right { get; set; }
    }


    [Authorize]
    public class CoreController : ApiController
    {

        public string UserId
        {
            get { return User.Identity.GetUserId(); }
        }

    }

    //Universal Controller
    [Authorize]
    public class CoreController<T> : ApiController where T : class
    {
        //public DbSet DBSet { get; set; }
        public BaseModelContext<T> DB { get; set; } = new BaseModelContext<T>();
        public bool CanCreate { get; set; } = true;
        public bool CanDelete { get; set; } = false;

        /// <summary>
        /// Returns a list of records based on OData query options.
        /// GET: Area/Controller/API/
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Json Response</returns>
        public virtual IHttpActionResult Get(ODataQueryOptions<T> options)
        {
            return Ok(new PageResult<T>(DB.Models, options));
        }
        
        [NonAction]
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
                    model = (T)DB.Models.Find(id);
                }
                catch
                {
                    return model;
                }
            }

            return model;
        }

        /// <summary>
        /// Gets the record from the associated DBSet
        /// GET: Area/Controller/API/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Json Response</returns>
        public virtual IHttpActionResult Get(string id)
        {
            T model = GetOne(id);
            
            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }


        /// <summary>
        /// Updates the record based on the PUT model
        /// PUT: Area/Controller/API/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public virtual IHttpActionResult Put(string id, T model)
        {
            var entity = GetOne(id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                DB.Entry(entity).CurrentValues.SetValues(model);
                DB.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(model);
        }


        public virtual IHttpActionResult Put(string id, string method, JObject jsonObject)
        {   //Override this method
            return Ok();
        }
        //[ResponseType(typeof(T))]
        /// <summary>
        /// Creates a new record based on the POSTed model 
        /// POST: Area/Controller/API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual IHttpActionResult Post(T model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            DB.Models.Add(model);
            DB.SaveChanges();
            
            return Ok();
        }

        // DELETE: Areas/Controller/API/5
        //[ResponseType(typeof(CustomerIssueModel))]
        /// <summary>
        /// Deletes the record from the db
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Json Response</returns>
        public virtual IHttpActionResult Delete(string id)
        {
            if (!CanDelete)
            {
                return NotFound();
            }
            T model = GetOne(id);
           
            if (model == null)
            {
                return NotFound();
            }
            DB.Models.Remove(model);
            DB.SaveChanges();
            return Ok(model);
        }

        //ModelExists string version
        /// <summary>
        /// ModelExists checks the existence of the record in the db
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public bool ModelExists(string id)
        {
            return GetOne(id) != null ? true : false;
        }

        [NonAction]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DB.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Provides the user id of the current user.
        /// </summary>
        /// 
        public string UserId
        {
            get { return User.Identity.GetUserId(); }
        }

        [NonAction]
        public void GetFilters(SingleValueNode node, List<ODataFilter> values)
        {
            if (node is BinaryOperatorNode)
            {
                var bon = (BinaryOperatorNode)node;
                var left = bon.Left;
                var right = bon.Right;

                if (left is ConvertNode)
                {
                    var convLeft = ((ConvertNode)left).Source;

                    if (convLeft is SingleValuePropertyAccessNode && (right is ConvertNode || right is ConstantNode))
                        ProcessConvertNode((SingleValuePropertyAccessNode)convLeft, right, bon.OperatorKind, values);
                    else
                        GetFilters(((ConvertNode)left).Source, values);
                }

                if (left is BinaryOperatorNode)
                {
                    GetFilters(left, values);
                }

                if (right is BinaryOperatorNode)
                {
                    GetFilters(right, values);
                }

                if (right is ConvertNode)
                {
                    GetFilters(((ConvertNode)right).Source, values);
                }


                if (left is SingleValuePropertyAccessNode && (right is ConvertNode || right is ConstantNode))
                {
                    ProcessConvertNode((SingleValuePropertyAccessNode)left, right, bon.OperatorKind, values);
                }
            }
        }

        [NonAction]
        public void ProcessConvertNode(SingleValuePropertyAccessNode left, SingleValueNode right, BinaryOperatorKind opKind, List<ODataFilter> values)
        {
            if (left is SingleValuePropertyAccessNode && (right is ConvertNode || right is ConstantNode))
            {
                var p = (SingleValuePropertyAccessNode)left;
                string value = "";
                if (right is ConstantNode)
                {
                    value = ((ConstantNode)right).Value.ToString();
                }
                if (right is ConvertNode)
                {
                    value = ((ConstantNode)((ConvertNode)right).Source).Value.ToString();
                }

                values.Add(new ODataFilter()
                {
                    Left = p.Property.Name,
                    Operator = opKind.ToString(),
                    Right = value
                }
                );

            }
        }
    }
}