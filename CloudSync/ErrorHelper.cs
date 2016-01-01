using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AndroAdminDataAccess.DataAccess;
using CloudSync;

namespace AndroAdmin.Helpers
{
    public class ErrorHelper
    {
        public static void LogError(string severity, string method, Exception exception)
        {
            try
            {
                // Get an object that can talk to the database
                ILogDAO logDAO = AndroAdminDataAccessFactory.GetLogDAO();

                // Create the log
                AndroAdminDataAccess.Domain.Log log = new AndroAdminDataAccess.Domain.Log();
                log.Source = "AndroAdmin";
                log.Method = method;
                log.Severity = severity;
                log.StoreId = "";
                log.Created = DateTime.UtcNow;

                if (exception != null)
                {
                    log.Message = exception.ToString();
                    if (log.Message.Length > 3499)
                    {
                        log.Message = log.Message.Substring(0, 3499);
                    }
                }

                // Add the log
                logDAO.Add(log);
            }
            catch { }
        }
    }
}