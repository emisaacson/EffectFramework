using HRMS.Modules.DBModel;
using HRMS.Modules.DBModel.Local;
using HRMS.CoreControllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HRMS.Core.Helpers
{
    public class DBHelper
    {
        public static EntityMaster GetCurrentEntity()
        {
            if (!HttpContext.Current.Items.Contains("HRMSLocalConfig"))
            {
                using (var Master = GetMasterDB())
                {
                    Master.DeferredLoadingEnabled = false;

                    if (DBHelper.IsSelfService() == true)
                    {
                        // return entity config for user depends on his ADUserID and ADDomain
                        var SplitUserName = GetSplitUserName();
                        var empMaster = Master.EmployeeMasters.FirstOrDefault(x => x.ADDomain.ToLower() == SplitUserName[0].ToLower()
                            && x.ADUserID.ToLower() == SplitUserName[1].ToLower() && x.IsActive == 'Y');
                        if (empMaster != null)
                        {
                            string sourceDatabaseName = empMaster.SourceDatabaseName.Substring(1, empMaster.SourceDatabaseName.Length - 2);
                            var Entity = Master.EntityMasters.FirstOrDefault(x => x.DBName.ToLower() == sourceDatabaseName.ToLower() && x.IsActive == 'Y');
                            if (Entity != null)
                            {
                                HttpContext.Current.Items["HRMSCurrentEntity"] = Entity;
                                return Entity;
                            }
                            else
                            {
                                throw new ConfigurationErrorsException("Database resolution error.");
                            }
                        }
                        else
                        {
                            throw new ConfigurationErrorsException("Database resolution error.");
                        }
                    }
                    else
                    {
                        EntityMaster Entity;
#if DEBUG
                        var DebugDatabase = ConfigurationManager.AppSettings["DebugDatabase"];
                        if (!string.IsNullOrWhiteSpace(DebugDatabase))
                        {
                            Entity = Master.EntityMasters.Where(x => x.DBName.ToLower() == DebugDatabase.ToLower() && x.IsActive == 'Y').FirstOrDefault();

                            if (Entity != null)
                            {
                                HttpContext.Current.Items["HRMSCurrentEntity"] = Entity;
                                return Entity;
                            }
                            else
                            {
                                // We want to bail here rather than risk using the wrong database.
                                throw new ConfigurationErrorsException("Database resolution error.");
                            }
                        }
#endif
                        var CurrentRequest = HttpContext.Current.Request;
                        string HttpHost = null;

                        // In production this sits behind a reverse proxy
                        if (CurrentRequest.ServerVariables["HTTP_X_ORIGINAL_HOST"] != null)
                        {
                            HttpHost = CurrentRequest.ServerVariables["HTTP_X_ORIGINAL_HOST"];
                        }
                        else
                        {
                            HttpHost = CurrentRequest.ServerVariables["HTTP_HOST"];
                        }
                        var EntityUrlHttp = "http://" + HttpHost;
                        var EntityUrlHttps = "https://" + HttpHost;

                        Entity = Master.EntityMasters.Where(x => (x.HRMSUrl.ToLower() == EntityUrlHttp.ToLower() || x.HRMSUrl.ToLower() == EntityUrlHttps.ToLower()) && x.IsActive == 'Y').SingleOrDefault();

                        if (Entity != null)
                        {
                            HttpContext.Current.Items["HRMSCurrentEntity"] = Entity;
                            return Entity;
                        }
                        else
                        {

                            // We want to bail here rather than risk using the wrong database.
                            throw new ConfigurationErrorsException("Database resolution error.");
                        }

                    }
                }
            }


            return (EntityMaster)HttpContext.Current.Items["HRMSCurrentEntity"];
        }

        private static string[] GetSplitUserName(bool ignoreImpersonating = false)
        {
            string[] SplitUserName;
            string impersonateCookie = CookieHelper.ImpersonationUser;
            if (impersonateCookie != null && !ignoreImpersonating)
            {
                SplitUserName = impersonateCookie.Split('\\');
            }
            else
            {
                SplitUserName = HttpContext.Current.User.Identity.Name.Split('\\');
            }

#if DEBUG
            if (impersonateCookie == null || ignoreImpersonating)
            {
                switch (HttpContext.Current.User.Identity.Name.ToLower())
                {
                    case "emikek\\administrator":
                        SplitUserName = new string[] { "hrms", "webfrontend" };
                        break;
                    case "sms1\\aljones":
                        SplitUserName = new string[] { "sms1", "eisaacson" };
                        break;
                    case "NOTEBOOK\\Bruno Ligutti":
                        SplitUserName = new string[] { "sms1", "bligutti" };
                        break;
                    case "ids\\cepefagel":
                        SplitUserName = new string[] { "sms1", "vkiryanov" };
                        break;
                    case "vitalii-pc\\vitalii":
                        SplitUserName = new string[] { "sms1", "vkiryanov" };
                        break;
                }
            }
#endif
            return SplitUserName;
        }

        public static bool IsSelfService()
        {
            return ConfigurationManager.AppSettings["SelfService"] != null &&
                   ConfigurationManager.AppSettings["SelfService"].ToString().ToLower() == "true";
        }

        public static bool IsUseETAS()
        {
            return ConfigurationManager.AppSettings["useETAS"] != null &&
                   ConfigurationManager.AppSettings["useETAS"].ToString().ToLower() == "true";
        }

        public static int GetEntityMasterID()
        {
            if (!HttpContext.Current.Items.Contains("EntityMasterID"))
            {
                using (var Master = GetMasterDB())
                {
                    var CurrentDatabase = GetCurrentDatabase();
                    HttpContext.Current.Items["EntityMasterID"] = Master.EntityMasters
                                                                        .Where(m => m.DBName.Equals(CurrentDatabase))
                                                                        .Select(e => e.EntityMasterID)
                                                                        .First();
                }
            }

            return (int)HttpContext.Current.Items["EntityMasterID"];
        }

        public static int GetCurrentEmpMasterId()
        {
            if (!HttpContext.Current.Items.Contains("EmpMasterId"))
            {
                using (var Master = GetMasterDB())
                {
                    var SplitUserName = GetSplitUserName();
                    var empMaster = Master.EmployeeMasters.FirstOrDefault(x => x.ADDomain.ToLower() == SplitUserName[0].ToLower()
                            && x.ADUserID.ToLower() == SplitUserName[1].ToLower());
                    int EmpMasterId = 0;
                    if (empMaster != null)
                        EmpMasterId = empMaster.EmpMasterID;
                    HttpContext.Current.Items["EmpMasterId"] = EmpMasterId;
                }
            }

            return (int)HttpContext.Current.Items["EmpMasterId"];
        }

        //public static HRMS.Modules.DBModel.HRMS.CoreUser GetAlwaysRealUser()
        //{
        //    try
        //    {
        //        using (var Local = LocalDataClassesDataContext.CreateWithExternalConnection(ConfigurationManager.ConnectionStrings["HRMS_LocalConnectionString"].ConnectionString))
        //        {
        //            var SplitUserName = GetSplitUserName(true);
        //            var LocalUser = HRMS.CoreUsers.Where(x => x.ADUser.ToLower() == SplitUserName[1].ToLower() &&
        //                                                       x.Domain.ToLower() == SplitUserName[0].ToLower() && x.IsActive == 'Y')
        //                                           .FirstOrDefault();
        //            return LocalUser;
        //        }
        //    }
        //    catch (ArgumentException)
        //    {
        //        return null;
        //    }
        //}

        public static bool IsEmployeeCanImpersonateInSSMode()
        {
            using(var Master = GetMasterDB())
            {
                return Master.ReviewUser.Any(x => x.Permission == "Impersonate" && x.EmpMasterId == GetCurrentEmpMasterId() && x.IsDeleted==false);
            }

        }

        public static UserContext GetCurrentUserContext(bool ignoreImpersonation=false)
        {
            if (!HttpContext.Current.Items.Contains("CurrentUser") || ignoreImpersonation)
            {
                try
                {
                    if (DBHelper.IsSelfService())
                    {
                        using (var Master = GetMasterDB())
                        {
                            var SplitUserName = GetSplitUserName(ignoreImpersonation);
                            if (SplitUserName[0] == "")
                                return null;
                            var CurrentEmployee = Master.EmployeeMasters.FirstOrDefault(x => x.ADUserID.ToLower() == SplitUserName[1].ToLower() &&
                                                               x.ADDomain.ToLower() == SplitUserName[0].ToLower() && x.IsActive == 'Y');
                            if (CurrentEmployee != null)
                            {
                                UserContext User = UserContext.GetUserContextFromEmployeeMaster(CurrentEmployee);

                                HttpContext.Current.Items["CurrentUser"] = User;
                                if (DBHelper.IsUseETAS() && HttpContext.Current.Session!=null)
                                {
                                    HrSelf.UserDetails ud = new HrSelf.UserDetails();
                                    Helper.FillInUserDetails(ref ud, CurrentEmployee.ADUserID);
                                    HttpContext.Current.Session["userDetails"] = ud;
                                }
                                return User;
                            }
                            else
                                throw new ConfigurationErrorsException("User does not exist.");

                        }
                    }
                    else
                    {
                        using (var Local = GetLocalDB())
                        {
                            var SplitUserName = GetSplitUserName(ignoreImpersonation);
                            if (SplitUserName[0] == "")
                                return null;
                            var LocalUser = HRMS.CoreUsers.Where(x => x.ADUser.ToLower() == SplitUserName[1].ToLower() &&
                                                                   x.Domain.ToLower() == SplitUserName[0].ToLower() && x.IsActive == 'Y')
                                                       .FirstOrDefault();

                            if (LocalUser != null)
                            {
                                HttpContext.Current.Items["CurrentUser"] = UserContext.GetUserContextFromLocalUser(LocalUser);
                            }
                            else
                            {

                                throw new ConfigurationErrorsException("User does not exist.");
                            }
                        }
                    }
                }
                catch (ArgumentException)
                {
                    throw new ConfigurationErrorsException("User does not exist.");
                }
            }

            return (UserContext)HttpContext.Current.Items["CurrentUser"];
        }

        public static LocalDataClassesDataContext GetLocalDB(string databaseName = null)
        {
            var ConnectionString = ConfigurationManager.ConnectionStrings["HRMS_LocalConnectionString"].ConnectionString;

            string NewConnectionString = LocalDataClassesDataContext.GetTransformedConnectionString(ConnectionString, databaseName ?? GetCurrentDatabase());

            return LocalDataClassesDataContext.CreateWithExternalConnection(NewConnectionString);
        }

        public static string GetCurrentDatabase(bool AddBrackets = false)
        {

#if DEBUG
            string DebugDatabase;
            if (DBHelper.IsSelfService() == true)
            {
                //DebugDatabase = GetCurrentEntity().DBName;
                DebugDatabase = ConfigurationManager.AppSettings["DebugDatabase"];
            }
            else
            {
                DebugDatabase = ConfigurationManager.AppSettings["DebugDatabase"];
            }
            if (!string.IsNullOrWhiteSpace(DebugDatabase))
            {
                if (AddBrackets)
                {
                    return "[" + DebugDatabase + "]";
                }
                else
                {
                    return DebugDatabase;
                }
            }
#endif
            var Config = GetCurrentEntity();


            if (AddBrackets)
            {
                return "[" + Config.DBName + "]";
            }
            else
            {
                return Config.DBName;
            }
        }

        public static HrmsMasterDataClassesDataContext GetMasterDB()
        {
#if DEBUG
            string DebugMasterDatabase = ConfigurationManager.AppSettings["DebugMasterDatabase"];
            if (!HttpContext.Current.Items.Contains("HRMSMasterConfig") && DebugMasterDatabase != null)
            {
                
                string DebugConnString = HrmsMasterDataClassesDataContext.GetTransformedConnectionString(
                    System.Configuration.ConfigurationManager.ConnectionStrings["HRMS_MasterConnectionString"].ConnectionString,
                    DebugMasterDatabase);

                HttpContext.Current.Items["HRMSMasterConfig"] = DebugConnString;
            }
#endif

            if (!HttpContext.Current.Items.Contains("HRMSMasterConfig"))
            {
                using (var Admin = GetAdminDB())
                {
                    var CurrentRequest = HttpContext.Current.Request;
                    string HttpHost = null;

                    // In production this sits behind a reverse proxy
                    if (CurrentRequest.ServerVariables["HTTP_X_ORIGINAL_HOST"] != null)
                    {
                        HttpHost = CurrentRequest.ServerVariables["HTTP_X_ORIGINAL_HOST"];
                    }
                    else
                    {
                        HttpHost = CurrentRequest.ServerVariables["HTTP_HOST"];
                    }
                    var EntityUrlHttp = "http://" + HttpHost;
                    var EntityUrlHttps = "https://" + HttpHost;

                    HRMS.Modules.DBModel.Admin.EntityMaster Entity;

                    if (DBHelper.IsSelfService())
                    {
                        Entity = Admin.EntityMasters.Where(x => (x.SelfServiceUrl.ToLower() == EntityUrlHttp.ToLower() || x.SelfServiceUrl.ToLower() == EntityUrlHttps.ToLower()) && x.IsDeleted == false).FirstOrDefault();
                    }
                    else
                    {
                        Entity = Admin.EntityMasters.Where(x => (x.HRMSUrl.ToLower() == EntityUrlHttp.ToLower() || x.HRMSUrl.ToLower() == EntityUrlHttps.ToLower()) && x.IsDeleted == false).SingleOrDefault();
                    }

                    if (Entity != null)
                    {
                        string MasterDB = Entity.TenantMaster.MasterDBName;

                        string ConnString = HrmsMasterDataClassesDataContext.GetTransformedConnectionString(
                            System.Configuration.ConfigurationManager.ConnectionStrings["HRMS_MasterConnectionString"].ConnectionString,
                            MasterDB);

                        HttpContext.Current.Items["HRMSMasterConfig"] = ConnString;
                    }
                    else
                    {
                        throw new ConfigurationErrorsException("Database resolution error: " + HttpHost);
                    }
                }
            }

            return HrmsMasterDataClassesDataContext.CreateWithExternalConnection(ExternalConnString: HttpContext.Current.Items["HRMSMasterConfig"].ToString());
        }

        public static HRMS.Modules.DBModel.Admin.AdminDataClassesDataContext GetAdminDB()
        {
            return HRMS.Modules.DBModel.Admin.AdminDataClassesDataContext.CreateWithExternalConnection(ConfigurationManager.ConnectionStrings["AdminConnectionString"].ConnectionString);
        }

        public static int? GetCurrentEmpGeneralID(bool IgnoreImpersonation = false)
        {
            var SplitUserName = GetSplitUserName(IgnoreImpersonation);

            using (var Master = GetMasterDB()) {
                var Employee = Master.EmployeeMasters.Where(x => x.ADUserID.ToLower() == SplitUserName[1].ToLower() &&
                                                       x.ADDomain.ToLower() == SplitUserName[0].ToLower() && x.IsActive == 'Y')
                                           .FirstOrDefault();

                if (Employee != null)
                {
                    return Employee.SourceEmpGeneralId;
                }

                return null;
            }
        }
    }
}
