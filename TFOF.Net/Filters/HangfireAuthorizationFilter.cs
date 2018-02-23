using TFOF.Areas.Core.Attributes;
using Hangfire.Dashboard;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire.Annotations;
using System.Globalization;
using Hangfire.Common;
using Hangfire.Client;
using Hangfire.Server;
using Hangfire.Storage;
using System.Security.Cryptography;
using Hangfire;

namespace TFOF.Filters
{
    //public class HangfireAuthorizationFilter : IAuthorizationFilter
    //{
    //    public bool Authorize(IDictionary<string, object> owinEnvironment)
    //    {
    //        // In case you need an OWIN context, use the next line,
    //        // `OwinContext` class is the part of the `Microsoft.Owin` package.
    //        var context = new OwinContext(owinEnvironment);
    //        if(context.Authentication.User.Identity.IsAuthenticated && 
    //            context.Authentication.User.IsInRole(SiteRole.Administrators))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //}


    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private string[] _roles = { SiteRole.Administrators };
        public bool Authorize([NotNull] DashboardContext context)
        { 
            
            var httpContext = new OwinContext(context.GetOwinEnvironment());
            var result = _roles.Aggregate(false, (current, role) => current || httpContext.Authentication.User.IsInRole(role));
            return result;
          
        }
    }

    public class DisableMultipleQueuedItemsFilter : JobFilterAttribute, IClientFilter, IServerFilter
    {
        private static readonly TimeSpan LockTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan FingerprintTimeout = TimeSpan.FromHours(1);

        public void OnCreating(CreatingContext filterContext)
        {
            if (!AddFingerprintIfNotExists(filterContext.Connection, filterContext.Job))
            {
                filterContext.Canceled = true;
            }
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            if (filterContext.Exception == null || filterContext.ExceptionHandled)
            {
                RemoveFingerprint(filterContext.Connection, filterContext.BackgroundJob.Job);
            }
        }


        private static bool AddFingerprintIfNotExists(IStorageConnection connection, Job job)
        {
            using (connection.AcquireDistributedLock(GetFingerprintLockKey(job), LockTimeout))
            {
                var fingerprint = connection.GetAllEntriesFromHash(GetFingerprintKey(job));

                DateTimeOffset timestamp;

                if (fingerprint != null &&
                    fingerprint.ContainsKey("Timestamp") &&
                    DateTimeOffset.TryParse(fingerprint["Timestamp"], null, DateTimeStyles.RoundtripKind, out timestamp) &&
                    DateTimeOffset.UtcNow <= timestamp.Add(FingerprintTimeout))
                {
                    // Actual fingerprint found, returning.
                    return false;
                }

                // Fingerprint does not exist, it is invalid (no `Timestamp` key),
                // or it is not actual (timeout expired).
                connection.SetRangeInHash(GetFingerprintKey(job), new Dictionary<string, string>
            {
                { "Timestamp", DateTimeOffset.UtcNow.ToString("o") }
            });

                return true;
            }
        }

        private static void RemoveFingerprint(IStorageConnection connection, Job job)
        {
            using (connection.AcquireDistributedLock(GetFingerprintLockKey(job), LockTimeout))
            using (var transaction = connection.CreateWriteTransaction())
            {
                transaction.RemoveHash(GetFingerprintKey(job));
                transaction.Commit();
            }
        }

        private static string GetFingerprintLockKey(Job job)
        {
            return String.Format("{0}:lock", GetFingerprintKey(job));
        }

        private static string GetFingerprintKey(Job job)
        {
            return String.Format("fingerprint:{0}", GetFingerprint(job));
        }

        //private static string GetFingerprint(Job job)
        //{
        //    string parameters = string.Empty;
        //    if (job.Arguments != null)
        //    {
        //        parameters = string.Join(".", job.Arguments);
        //    }
        //    if (job.Type == null || job.Method == null)
        //    {
        //        return string.Empty;
        //    }
        //    var fingerprint = String.Format(
        //        "{0}.{1}.{2}",
        //        job.Type.FullName,
        //        job.Method.Name, parameters);

        //    return fingerprint;
        //}


        private static string GetFingerprint(Job job)
        {
            var parameters = string.Empty;
            if (job?.Args != null)
            {
                parameters = string.Join(".", job.Args);
            }
            if (job?.Type == null || job.Method == null)
            {
                return string.Empty;
            }
            var payload = $"{job.Type.FullName}.{job.Method.Name}.{parameters}";
            var hash = SHA256.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));
            var fingerprint = Convert.ToBase64String(hash);
            return fingerprint;
        }

        void IClientFilter.OnCreated(CreatedContext filterContext)
        {
        }

        void IServerFilter.OnPerforming(PerformingContext filterContext)
        {
        }
    }
}
