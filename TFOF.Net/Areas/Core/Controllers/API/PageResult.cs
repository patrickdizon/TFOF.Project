namespace TFOF.Areas.Core.Controllers.API
{
    using Models;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Web.Http.OData.Query;
    
    public class PageResult<T>
    {   
        
        public PageResult()
        {
            //Do nothing
        }
        public PageResult(DbSet dbSet, ODataQueryOptions options, int maxPageSize = 200)
        {
            //prevent going over 200 items
            top = options.Top != null ? options.Top.Value : pageSize;
            top = (top >= maxPageSize) ? maxPageSize : top;
            ODataQuerySettings settings = new ODataQuerySettings()
            {
                PageSize = top
            };
            
            Items = options.ApplyTo(dbSet.AsQueryable(), settings);
            Count = GetCount(dbSet, options);
            Next = GetNextCount(dbSet, options);
            
        }

        public object Items; //Could be IQueryable or List
        public int Count;
        public int Next =  0;
        public List<StatModel> Stats;
        private const int pageSize = 30;
        private int top = 0;
        
        private string GetCountUrl(string url)
        {
            List<string> countExclusion = new List<string> {
                "(\\&\\$|\\$)top=\\d+"
                ,"(\\&\\$|\\$)skip=\\d+"
                ,"((\\&\\$|\\$)select=[/,a-zA-Z0-9]*)"
                ,"((\\&\\$|\\$)expand=[/,a-zA-Z0-9]*)"
                ,"((\\&\\$|\\$)orderby=[/,% a-zA-Z0-9]*)"
            };

            foreach (string pattern in countExclusion)
            {
                Regex rgx = new Regex(pattern);
                url = rgx.Replace(url, "");
            }
            return url;
        }

        private ODataQueryOptions CountOptions(ODataQueryOptions options)
        {
            var url = options.Request.RequestUri.AbsoluteUri;
            var request = new HttpRequestMessage(HttpMethod.Get, GetCountUrl(url));
            var countOptions = new ODataQueryOptions(options.Context, request);
            return countOptions;
        }

        private int GetNextCount(DbSet dbset, ODataQueryOptions options)
        {
            var url = options.Request.RequestUri.AbsoluteUri;
            var next =  (options.Skip != null ? options.Skip.Value + top : top);
            url = GetCountUrl(url) + "&$top=" + top.ToString() + "&$skip=" + next.ToString();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            return (new ODataQueryOptions(options.Context, request).ApplyTo(dbset.AsQueryable()) as IQueryable<T>).Count(); 
        }

        private int GetCount(DbSet dbset, ODataQueryOptions options)
        {
            return (CountOptions(options).ApplyTo(dbset.AsQueryable()) as IQueryable<T>).Count();
        }

        /// An alias
        /// <summary>
        /// Returns an ODataQueryOptions with the Filter properties only. Strips out Skip, Top, Select/Expand and OrderBy
        /// </summary>
        /// <param name="options">ODataQueryOptions</param>
        /// <returns></returns>

        public ODataQueryOptions FilterOnlyOptions(ODataQueryOptions options)
        {
            return CountOptions(options);
        } 
        
    }

}