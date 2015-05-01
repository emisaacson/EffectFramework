using HrSelf;
using HrSelf.Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Local.Classes.Helpers
{
    public static class Helper
    {
#if DEBUG
        public static bool bNotFirstLoad = false;
#endif
        public static string DefaultDatabase = "HRMS_SMSUS";

        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
				return false;
#endif
            }
        }

        public static string StringFirstStartDateETAS
        {
            get
            {
                // using slashes is better for javascript (dashes make it adjust for timezone)
                return "2013/04/01";	// the earliest possible date
            }
        }

        public static DateTime FirstStartDateETAS
        {
            get
            {
                return DateTime.Parse(StringFirstStartDateETAS);
            }
        }

        public static int FirstYearETAS
        {
            get
            {
                return 2013;
            }
        }

        public static UserDetails UserDetails
        {
            get
            {
                if (DBHelper.IsSelfService() && DBHelper.IsUseETAS())
                {
                    UserDetails ud = HttpContext.Current.Session["userDetails"] as UserDetails;
                    if (ud == null)
                    {
                        ud = new UserDetails { };
                        HttpContext.Current.Session["userDetails"] = ud;
                    }

                    // note that modifying ud updates the Session object too (they're one and the same!)
                    FillInUserDetails(ref ud, ud.ADUserId, ud.ADDomain, ud.IsImpersonatingEntity ? ud.EntityDb : null,
                        ud.IsImpersonatingUser ? ud.actualMe.aduserid : null, ud.aduseridImpersonatingTwice);

                    // usually this is saved when impersonating starts, but just in case it's not there, fetch it
                    if (ud.IsImpersonatingUser && ud.actualMe == null)
                    {
                        ud.actualMe = GetPerson();	// gets logged in user
                    }

#if DEBUG
                    if (!bNotFirstLoad)
                    {
                        bNotFirstLoad = true;
                        // check for auto impersonate the first time
                        var oAutoImpUser = System.Web.Configuration.WebConfigurationManager.AppSettings["AutoImpersonateUser"];
                        int nAutoImpUser;
                        if (oAutoImpUser != null && int.TryParse(oAutoImpUser.ToString(), out nAutoImpUser) && nAutoImpUser != 0)
                        {
                            var oAutoImpDomain = System.Web.Configuration.WebConfigurationManager.AppSettings["AutoImpersonateDomain"];
                            var oAutoImpUserId = System.Web.Configuration.WebConfigurationManager.AppSettings["AutoImpersonateUserId"];
                            var oAutoImpEntityDb = System.Web.Configuration.WebConfigurationManager.AppSettings["AutoImpersonateEntityDb"];
                            var result = new HrSelf.Controllers.HomeController().Impersonate(oAutoImpDomain, oAutoImpUserId, oAutoImpEntityDb);
                            return UserDetails;
                        }
                    }
#endif

                    return ud;
                }
                else
                    return new UserDetails { };
            }
        }

        public static void FillInUserDetails(ref UserDetails ud, string aduserid, string domain = null, string entitydb = null,
            string aduseridImpersonator = null, string aduseridTwiceImpersonator = null)
        {
            if (ud.me == null || ud.me.EmpMasterID == 0)
            {
                ud.me = GetPerson(domain, aduserid, entitydb, aduseridImpersonator, aduseridTwiceImpersonator);
                if (ud.me != null && string.IsNullOrEmpty(ud.ADUserId))
                {
                    // save off the domain/user since it used the logged in user above
                    ud.ADDomain = !string.IsNullOrEmpty(ud.me.addomain) ? ud.me.addomain.ToLower() : null;
                    ud.ADUserId = !string.IsNullOrEmpty(ud.me.aduserid) ? ud.me.aduserid.ToLower() : null;
                }
                else if (string.IsNullOrEmpty(ud.ADDomain) && ud.me != null)
                {
                    // we searched without a domain (impersonated user)
                    ud.ADDomain = !string.IsNullOrEmpty(ud.me.addomain) ? ud.me.addomain.ToLower() : null;
                    ud.RedoUserIdentity();
                }
            }

            if (!ud.EmpMasterId.HasValue)
            {
                if (ud.me != null && ud.me.EmpMasterID != 0)
                {
                    ud.EmpGeneralId = ud.me.SourceEmpGeneralId.Value;
                    ud.EmpMasterId = ud.me.EmpMasterID;
                    ud.EntityName = ud.me.EntityName;
                    ud.EntityDb = ud.me.SourceDatabaseName.Replace("[", "").Replace("]", "");
                }
            }
        }

        // Returns true if all required sections are filled out to submit for HR for approval. Provides a
        // list of text validation errors if not all sections are filled out.
        public static bool CanUserSubmitRecordToHR(int EmpMasterID, out List<string> ErrorMessages)
        {
            using (var Master = Helper.GetHrMasterDb())
            {
                var Issues = Master.usp_SelfService_EmployeeSummary(EmpMasterID, 1);

                List<string> Messages = new List<string>();
                foreach (var Issue in Issues)
                {
                    var section = Issue.Section;

                    if (section == "Address Information")
                    {
                        section = "Primary Address Information";
                    }

                    Messages.Add(section + " (" + Issue.Value + ")");
                }
                ErrorMessages = Messages.Distinct().ToList();

                if (Messages.Count > 0)
                {
                    return false;
                }
                else
                {
                    if (HttpContext.Current.Session["CameFromPM"] as bool? == true &&
                        HttpContext.Current.Session["ValidatedPages"] != null)
                    {
                        if (((PagesValidatedByEmployee)HttpContext.Current.Session["ValidatedPages"]).AllPagesAreValidated())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

        }

        public static void SendHRMail()
        {
            string HRMSLocalUrl = null;

            using (var Master = Helper.GetHrMasterDb())
            {
                var DBName = Helper.UserDetails.EntityDb;
                var CurrentEntity = Master.EntityMasters.Where(x => x.IsActive == 'Y' && x.DBName == DBName).FirstOrDefault();
                if (CurrentEntity != null)
                {
                    HRMSLocalUrl = CurrentEntity.HRMSUrl;
#if DEBUG
                    var to = "jhanko@starmanagement.net;eisaacson@starmanagement.net";
#else
                    string to = CurrentEntity.HRDistributionList;
#endif
                    if (to != null)
                    {
                        var Person = Helper.Person;
                        var Email = new EmailLog()
                        {
                            Subject = "New Hire " + Person.Employee + " has completed submitting their self-service record.",
                            Body = Person.Employee + @" has completed submitting their self service record. Please visit " + HRMSLocalUrl + "/Employee/" + Person.EmpMasterID.ToString() + " to complete HR review of the record.",
                            SentTo = to,
                            SentFrom = "noreply@atsgroupltd.com",
                            TimeSent = DateTime.Now
                        };

                        Mailer.SendMail(Email.SentFrom, Email.SentTo, Email.Subject, Email.Body);

                        Master.EmailLogs.InsertOnSubmit(Email);
                        Master.SubmitChanges();
                    }
                }
            }
        }

        // special case on login
        public static void ResetUserDetails(string domain, string user)
        {
            UserDetails ud = new UserDetails
            {
                ADDomain = !string.IsNullOrEmpty(domain) ? domain.ToLower() : null,
                ADUserId = !string.IsNullOrEmpty(user) ? user.ToLower() : null
            };
            ClearSiteConfig(ud);
        }

        // special case on impersonate
        public static void ImpersonateUserDetails(UserDetails ud)
        {
            ClearSiteConfig(ud);
        }

        public static SiteConfig GetSiteConfig(ref TimeKeeperDataClassesDataContext tk, bool bForceTK = false, bool bForceEntities = false)
        {
            bool bNew = false;
            bool bGetEntities = bForceEntities;
            bool bCheckETAS = false;

            if (tk == null && bForceTK)
            {
                tk = Helper.GetContextTimeKeeper();
            }

            // note that modifying sc updates the Session object too (they're one and the same!)
            SiteConfig sc = HttpContext.Current.Session != null && HttpContext.Current.Session["siteConfig"] is SiteConfig ? HttpContext.Current.Session["siteConfig"] as SiteConfig : null;
            if (sc == null || (sc.dtUtcExpires <= DateTime.UtcNow))
            {
                var ud = UserDetails;	// pull info out of Session
                sc = new SiteConfig
                {
                    dtUtcExpires = DateTime.UtcNow.AddHours(2),
                    ud = ud
                };

#if DEBUG_NOTYET
				var oSuperUser = System.Web.Configuration.WebConfigurationManager.AppSettings["DebugSuperUser"];
				int nSuperUser;
				sc.dbgIsSuperUser = (oSuperUser != null && int.TryParse(oSuperUser.ToString(), out nSuperUser) && nSuperUser != 0);

				// don't cache this since it's a pain when restarting with web.config change
				var oNoLocking = System.Web.Configuration.WebConfigurationManager.AppSettings["DebugNoLocking"];
				int nNoLocking;
				sc.dbgIsNoLocking = (oNoLocking != null && int.TryParse(oNoLocking.ToString(), out nNoLocking) && nNoLocking != 0)
#endif

                if (ud.me != null)
                {
                    sc.canImpersonate = ud.me.IsImpersonator;
                    bool? IsEtasManager = ud.me.IsEtasManager;
                    if (IsEtasManager.HasValue)
                    {
                        sc.isEtasManager = IsEtasManager.Value;
                    }
                }

                bGetEntities = true;

                HttpContext.Current.Session["siteConfig"] = sc;
                bNew = true;
            }

            //bool bAuth = HttpContext.Current.Request.IsAuthenticated;
            bool bAuth = (sc.ud.me != null && sc.ud.me.EmpMasterID > 0);	// i.e. is authenticated
            if (bNew || (bAuth && !sc.bCheckedPostAuth))
            {
#if DEBUG
                if (sc.canImpersonate)
                {
                    sc.canLoginTestUsers = true;
                }
#endif

                if (sc.settings == null)
                {
                    sc.settings = HrSelf.Common.GetSettings(ref tk);
                }

                sc.bNoTimesheets = sc.GetBoolSetting("NoTimesheets");
                int nETASRulesMode = sc.GetIntSetting("ETASRulesMode");
                sc.bFinanceMode = (nETASRulesMode & (int)ETASRulesMode.Finance) != 0;
                sc.bNoPriorData = sc.GetBoolSetting("NoPriorData");
                sc.startDateEtas = sc.GetDateTimeSettingNotNull("StartDateETAS", Helper.StringFirstStartDateETAS);

                /*
                // no longer meaningful with single site and ntlm authentication
                if (sc.settings.Count == 0)
                {
                    // special case where someone logs in on non-etas site (e.g. Logic) because they manage a team
                    sc.bNoTimeOff = true;
                    sc.bNoTimesheets = true;
                }
                */

                if (bAuth)
                {
                    sc.bAllowEmployeeETAS = sc.IsEmployeeTypeAllowed(sc.ud.me.EmpType);
                    if (!sc.bAllowEmployeeETAS)
                    {
                        sc.bNoTimeOff = true;
                        sc.bNoTimesheets = true;
                    }

                    if (tk == null)
                    {
                        tk = Helper.GetContextTimeKeeper();
                    }

                    if (sc.es == null)
                    {
                        sc.es = new EmployeeSetting();

                        // check if employee exists in TK without creating them
                        int? tkId = sc.ud.GetTimekeeperId(ref tk, true);
                        if (tkId.HasValue)
                        {
                            // need the setting
                            if (tk == null)
                            {
                                tk = Helper.GetContextTimeKeeper();
                            }
                            var empinfo = tk.usp_EmployeeInfo(tkId.Value).FirstOrDefault();
                            if (empinfo != null)
                            {
                                var setting = empinfo.EmployeeSetting;
                                if (setting != null)
                                {
                                    sc.es = setting;
                                }
                            }

                            var mgrs = tk.EtasManagers.Where(x => x.ManagerId == tkId.Value && x.IsDeleted == false).ToList();
                            sc.listHandleTimeoff = mgrs.Where(x => x.HandleTimeoff == true).Select(x => x.EmployeeId).ToList();
                            sc.listHandleTimesheet = mgrs.Where(x => x.HandleTimesheet == true).Select(x => x.EmployeeId).ToList();

                            var divisions = tk.DivisionManagers.Where(x => x.ManagerId == tkId.Value && x.IsDeleted == false).ToList();
                            sc.bDivisionManager = divisions.Count > 0;
                        }
                    }

                    sc.bCheckedPostAuth = true;
                }

                sc.hasProxy = sc.ud.me != null && sc.ud.me.ProxyForEmployeeId.HasValue && sc.ud.me.ProxyForEmployeeId.Value > 0;
                sc.cProxyForManager = 0;
                if (sc.hasProxy)
                {
                    if (tk == null)
                    {
                        tk = Helper.GetContextTimeKeeper();
                    }
                    sc.proxies = tk.EmployeeProxies.Where(x => x.ProxyAsEmployeeId == sc.ud.TimekeeperId && x.IsDeleted == false).ToList();
                    sc.cProxyForManager = sc.proxies.Count(x => x.ManageSchedule == true);
                }

                /*
                // redundant now that it always gets set below
                if (sc.bAllowEmployeeETAS || sc.canLoginTestUsers)
                {
                    bGetEntities = true;
                }
                */
                bGetEntities = true;
                bCheckETAS = true;
            }

            if (bGetEntities && sc.entities == null)
            {
                using (var master = Helper.GetHrMasterDb())
                {
                    // get all the entities that use ETAS (independent of employee)
                    sc.entities = master.usp_Entities("ETAS").ToList();
                }
            }

            if (sc.ShortEntities == null)
            {
                sc.ResetAllThisUsersEntities(ref tk);
            }

            if (bCheckETAS && sc.ud.EntityDb != null && (!sc.entities.Exists(x => x.DBName.ToLower() == sc.ud.EntityDb.ToLower())
                // || !sc.ud.TimekeeperId.HasValue
                ))
            {
                // user is not in an ETAS-using entity
                sc.bNoTimeOff = true;
                sc.bNoTimesheets = true;
            }

            /*
            if (bAuth && !sc.ud.TimekeeperId.HasValue)
            {
                // if accessing an ETAS page, make sure that TimekeeperId exists or is created
                //$BUGBUG: need a condition here
                if (false)
                {
                    // calling this sets the TimekeeperId in ud for us
                    sc.ud.GetTimekeeperId(ref tk);
                }
            }
            */

            return sc;
        }

        public static void ClearSiteConfig(UserDetails udSet = null)
        {
            HttpContext.Current.Session["userDetails"] = udSet;
            HttpContext.Current.Session["siteConfig"] = null;
        }

        public static usp_PersonResult Person
        {
            get
            {
                var ud = UserDetails;
                return ud.me;
            }
        }

        public static void ParseNameDomain(string identityName, out string user, out string domain)
        {
            var name = identityName;
            if (name.Contains("\\"))
            {
                user = name.Split('\\')[1];
                domain = name.Split('\\')[0];
            }
            else
            {
                user = name;
                domain = null;
            }
        }

        public static string EntityName
        {
            get
            {
                return UserDetails.EntityName;
            }
        }

#if NOT_USED_ANYMORE
		public static string CompanyName
		{
			get
			{
				var ud = UserDetails;
				if (!string.IsNullOrEmpty(ud.EntityName))
				{
					return UserDetails.EntityName;
				}
				HrmsDataClassDataContext ctx = null;
				var config = GetConfig(null, ref ctx);
				return config != null ? config.Company : "";
			}
		}
#endif

        public static usp_GetConfigResult GetConfig(string serverName, ref HrmsDataClassDataContext ctx)
        {
            usp_GetConfigResult config = null;

            if (ctx == null)
            {
                ctx = Helper.GetContext();
            }

            if (string.IsNullOrEmpty(serverName))
            {
                serverName = HttpContext.Current.Request.ServerVariables.Get("SERVER_NAME");
            }

            string serverNameLC = serverName.ToLower();
            if (serverNameLC == "localhost")
            {
                serverNameLC = "10.7.10.105";
            }

            var configs = ctx.usp_GetConfig("HrSelf", null, null).ToList();

            var ud = UserDetails;
            if (config == null && !string.IsNullOrEmpty(ud.EntityDb))
            {
                // look up by dbname (might be impersonating)
                config = configs.Where(x => x.DbName.ToLower() == ud.EntityDb.ToLower() && x.IsActive == true).FirstOrDefault();
            }
#if DEBUG
            if (config == null)
            {
                string currentDatabase = Common.CurrentDatabase;
                if (!string.IsNullOrEmpty(currentDatabase))
                {
                    // look up by dbname (might be impersonating)
                    config = configs.Where(x => x.DbName.ToLower() == currentDatabase.ToLower() && x.IsActive == true).FirstOrDefault();
                }
            }
#endif
            if (config == null)
            {
                // look up by url (ignore if multiple)
                var configsT = configs.Where(x => x.URL.ToLower() == serverNameLC && x.IsActive == true).ToList();
                if (configsT.Count <= 1)
                {
                    config = configsT.SingleOrDefault();
                }
            }

#if DEBUG
            if (config == null)
            {
                config = new usp_GetConfigResult
                {
                    Company = "Dev",
                    EntityName = "Dev",
                    DbName = DefaultDatabase,
                    //DbServer = "localhost",
                    IsActive = true
                };
            }
#endif
            if (config != null)
            {
                config.ServerName = serverNameLC;
            }
            return config;
        }

        public static void Log(string activity, string url = null, string extra = null, int? empIdMaybeViewAs = null, usp_PersonResult meForce = null)
        {
            var ud = Helper.UserDetails;	// might be impersonating
            HrSelf.UserDetails udViewAs = null;

            var db = GetContextTimeKeeper();

            if (empIdMaybeViewAs.HasValue)
            {
                if (!ud.TimekeeperId.HasValue || empIdMaybeViewAs.Value != ud.TimekeeperId.Value)
                {
                    // must be doing something to a team member
                    string aduserid = db.Employees.Where(x => x.EmployeeId == empIdMaybeViewAs.Value).Select(x => x.AdUserId).FirstOrDefault();
                    var empinfo = db.Employees.Where(x => x.EmployeeId == empIdMaybeViewAs.Value).Select(x => new { aduserid = x.AdUserId, entitydb = x.EntityDb }).FirstOrDefault();
                    if (!string.IsNullOrEmpty(empinfo.aduserid))
                    {
                        udViewAs = new HrSelf.UserDetails { };
                        FillInUserDetails(ref udViewAs, empinfo.aduserid, null, empinfo.entitydb);
                    }
                }
            }

            var result = new ActivityLog()
            {
                CreateDate = DateTime.Now,
                CreateDateUtc = DateTime.UtcNow,
                Activity = activity,
                UserIdentity = udViewAs != null ? udViewAs.UserIdentity : ud.UserIdentity,
                //DbName = Common.CurrentDatabase,
                DbName = udViewAs != null ? udViewAs.EntityDb : ud.EntityDb,	// this will be null if not logged in (and user manually went to an MVC page)
                //EmpGeneralId = Common.UserId,
                Host = HttpContext.Current.Request.ServerVariables.Get("SERVER_NAME")
            };

            if (udViewAs != null)
            {
                /*
                if (udViewAs.TimekeeperId.HasValue)
                {
                    //result.EmployeeId = udViewAs.TimekeeperId.Value;
                }
                 */
                result.EmployeeId = empIdMaybeViewAs.Value;

                if (udViewAs.EmpGeneralId.HasValue)
                {
                    result.EmpGeneralId = udViewAs.EmpGeneralId.Value;
                }

                if (udViewAs.me != null)
                {
                    result.EmpMasterId = udViewAs.me.EmpMasterID;
                }
            }
            else
            {
                if (ud.TimekeeperId.HasValue)
                {
                    result.EmployeeId = ud.TimekeeperId.Value;
                }

                if (ud.EmpGeneralId.HasValue)
                {
                    result.EmpGeneralId = ud.EmpGeneralId.Value;
                }

                if (ud.me != null)
                {
                    result.EmpMasterId = ud.me.EmpMasterID;
                }
            }

            if (meForce != null)
            {
                // special case where we want actual user to be the main user identity
                result.UserIdentity = meForce.UserIdentity;
                result.DbName = meForce.SourceDatabaseName.Replace("[", "").Replace("]", "");
                result.EmployeeId = meForce.GetTimekeeperId(ref db);
                result.EmpGeneralId = meForce.SourceEmpGeneralId;
                result.EmpMasterId = meForce.EmpMasterID;
            }
            else if (ud.IsImpersonatingUser)
            {
                // also log the real user who is logged in
                if (ud.actualMe != null)
                {
                    result.ActualUserIdentity = ud.actualMe.UserIdentity;
                }
                else
                {
                    result.ActualUserIdentity = Common.GetCurrentIdentityName();
                }
            }
            else if (udViewAs != null)
            {
                // also log the user who is logged in
                result.ActualUserIdentity = ud.UserIdentity;
            }

            if (!string.IsNullOrEmpty(url))
            {
                result.Url = url;
            }
            else
            {
                result.Url = HttpContext.Current.Request.ServerVariables.Get("URL");
            }

            if (!string.IsNullOrEmpty(extra))
            {
                result.Extra = extra;
            }

            db.ActivityLogs.InsertOnSubmit(result);
            db.SubmitChanges();
        }

        public static string EoPdfLicenseKey
        {
            get
            {
                return "S561n1mXpM0M66Xm+8+4iVmXpLHLn1mXwPIP41nr/QEQvFu807/745+ZpAcQ" +
        "8azg8//ooW2ltLPLrneEjrHLn1mzs/IX66juwp61n1mXpM0a8Z3c9toZ5aiX" +
        "6PIf5HaZt8DhrmuntcPNn6/c9gQU7qe0psTNn2i1kZvLn1mXwAQU5qfY+AYd" +
        "5Hfg1/rWuIjtx9/j9aPn1dT/tI7K2vUivHazswQU5qfY+AYd5HeEjs3a66La" +
        "6f8e5HeEjnXj7fQQ7azcwp61n1mXpM0X6Jzc8gQQyJ21uMPdsnGvvcTir3Wm" +
        "8PoO5Kfq6doPvUaBpLHLn3Xj7fQQ7azc6c/nrqXg5/YZ8p7cwg==";
            }
        }

        public static void CreateSelfServiceAudit(object src, object dest)
        {
            var db = GetContext();

            var srcProps = src.GetType().GetProperties();
            var destProps = dest.GetType().GetProperties();
            var cu = Common.GetCurrentPerson();
            string pkField = ""; int pkId = 0;
            try
            {
                pkField = db.Mapping.GetTable(dest.GetType()).RowType.IdentityMembers[0].Name;
            }
            catch { }

            var dppk = destProps.Where(x => x.Name == pkField).FirstOrDefault();
            if (dppk != null)
            {
                try
                {
                    pkId = Convert.ToInt32(dppk.GetValue(dest, null));
                }
                catch { }
            }
            var excludes = new string[] { "IsGlobalHr", "IsSelfCreated", "CreateDate", "CreatedBy", "UpdateDate", "UpdatedBy" };

            foreach (var srcProp in srcProps)
            {
                var srcName = srcProp.Name;
                if (excludes.Contains(srcName)) continue;
                var destProp = destProps.Where(x => x.Name == srcName).FirstOrDefault();
                if (destProp != null)
                {
                    // match, compare values
                    var srcVal = srcProp.GetValue(src, null);
                    var destVal = destProp.GetValue(dest, null);
                    var srcValStr = (srcVal ?? "").ToString();
                    var destValStr = (destVal ?? "").ToString();
                    //if (srcVal != null && destVal != null)
                    if (srcValStr != destValStr)
                    {
                        //if (!srcVal.Equals(destVal))
                        //{
                        var ssa = new SelfServiceAudit()
                        {
                            PrimaryKeyField = pkField,
                            PrimaryKeyId = pkId,
                            EmpGeneralId = cu.SourceEmpGeneralId,
                            EmpMasterId = cu.EmpMasterID,
                            FieldName = srcName,
                            OldValue = srcValStr, //.ToString(),
                            NewValue = destValStr, //.ToString(),
                            TableName = dest.ToString().Replace("HrSelf.", ""),
                            UpdateBy = Common.GetCurrentIdentityName(),
                            UpdateDate = DateTime.Now
                        };
                        db.SelfServiceAudits.InsertOnSubmit(ssa);
                        //}
                    }
                }
            }

            db.SubmitChanges();
        }

        public static string CurrentConnectionString
        {
            get
            {
                var ud = UserDetails;
                if (!string.IsNullOrEmpty(ud.ConnectionString))
                {
                    return ud.ConnectionString;
                }
                return ConfigurationManager.ConnectionStrings["HRMS_MasterConnectionString"].ConnectionString;
            }
        }

        public static string GetFileName(string fullPath)
        {
            if (fullPath.Contains("\\"))
            {
                var sp = fullPath.Split('\\');
                var name = sp[sp.Length - 1];
                return name;
            }
            else
            {
                return fullPath;
            }
        }

        public static byte[] StreamToBytes(System.IO.Stream stream)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            PushData(stream, ms);
            return ms.ToArray();
        }

        public static void PushData(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) != 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }

        public static string GetDatabaseName(string connectionString)
        {
            string sDatabase = "";
            Match match = Regex.Match(connectionString, @"Catalog=(.*?);");
            if (match.Success)
            {
                sDatabase = match.Groups[1].Captures[0].ToString();
            }
            return sDatabase;
        }

        public static bool IsUserGlobalHr(string name)
        {
            string[] globalHr = new string[] { @"agt\ckarr", @"sms1\ahystek", @"sms1\blyons", @"sms1\aarnon", @"agt\blyons", @"sms1\teisenmann", @"sms1\dfechner", @"sms1\sghanem", @"agt\sghanem" };
            string id = name.ToLower();
            return globalHr.Contains(id);
        }

        public static string Repeat(this char chatToRepeat, int repeat)
        {
            return new string(chatToRepeat, repeat);
        }
        public static string Repeat(this string stringToRepeat, int repeat)
        {
            var builder = new StringBuilder(repeat);
            for (int i = 0; i < repeat; i++)
            {
                builder.Append(stringToRepeat);
            }
            return builder.ToString();
        }

#if NOT_USED_ANYMORE	// and if it was used, it should use JobAssignMaster
		public static List<EmpJobAssignment> GetJobAssignmentsRecursively(int employeeId)
		{
			List<EmpJobAssignment> result = new List<EmpJobAssignment>();
			var ctx = Helper.GetContext();
			var eja = ctx.EmpJobAssignments.Where(x => x.DirectManager == employeeId && x.EmpID != employeeId).ToList();
			if (eja != null && eja.Count > 0)
			{
				result.AddRange(eja);
				for (int i = 0; i < result.Count; i++)
				{
					var r = result[i];
					result.AddRange(GetJobAssignmentsRecursively(r.EmpID));
				}
			}
			return result;
		}
#endif

        public static T FindControl<T>(Control startingControl, string id) where T : Control
        {
            T found = null;

            foreach (Control activeControl in startingControl.Controls)
            {
                found = activeControl as T;

                if (found == null)
                {
                    found = FindControl<T>(activeControl, id);
                }
                else if (string.Compare(id, found.ID, true) != 0)
                {
                    found = null;
                }

                if (found != null)
                {
                    break;
                }
            }

            return found;
        }

        public static HrSelf.usp_PersonResult GetPerson(string identityName)
        {
            string domain;
            string user;
            ParseNameDomain(identityName, out user, out domain);
            return GetPerson(domain, user);
        }

        public static HrSelf.usp_PersonResult GetPerson(string domain = null, string user = null, string entityImpersonate = null,
            string aduseridImpersonator = null, string aduseridTwiceImpersonator = null)
        {
            string identityName = null;

            if (string.IsNullOrEmpty(user) && string.IsNullOrEmpty(domain))
            {
                identityName = Common.GetCurrentIdentityName();
                if (!string.IsNullOrEmpty(identityName))
                {
#if DEBUG
                    if (identityName == "atkins8\\jon")
                    {
                        identityName = "sms1\\jatkins";
                    }
                    else if (identityName.ToLower() == "emikek\\administrator" || identityName.ToLower() == "emikek\\elliot")
                    {
                        identityName = "sms1\\eisaacson";
                    }
                    else if (identityName.ToLower() == "sms1\\aljones")
                    {
                        identityName = "sms1\\eisaacson";
                    }
                    else if (identityName.ToLower() == "vitalii-pc\\vitalii")
                    {
                        identityName = "sms1\\egarrett";
                    }
                    else if (identityName.ToLower() == "notebook\\bruno ligutti")
                    {
                        identityName = "sms1\\aarnon";
                    }
                    //identityName = "sms1\\DStackiewicz";
                    if (identityName.ToLower() == "sms1\\vkiryanov")
                    {
                        identityName = "sms1\\egarrett";
                    }
                    if (identityName.ToLower() == "cr\\быкова")
                    {
                        identityName = "sms1\\eisaacson";
                    }
#endif
                    ParseNameDomain(identityName, out user, out domain);
                }
            }

            if (string.IsNullOrEmpty(user))// || string.IsNullOrEmpty(domain))
            {
                return null;
            }

            if (string.IsNullOrEmpty(domain))
            {
                domain = null;		// make it work to pick the first one
            }

            string impersonatorForPrivs = !string.IsNullOrEmpty(aduseridTwiceImpersonator) ? aduseridTwiceImpersonator : aduseridImpersonator;
            var person = GetHrMasterDb().usp_Person(domain, user, null, entityImpersonate, null, false, impersonatorForPrivs).FirstOrDefault();
            return person;
        }

        public static HrmsDataClassDataContext GetContext()
        {
            string conStr = CurrentConnectionString;
            if (!string.IsNullOrEmpty(conStr))
            {
                return new HrmsDataClassDataContext(conStr);
            }
            return new HrmsDataClassDataContext();
        }

        public static HrMasterDataClassesDataContext GetHrMasterDb()
        {
            var context = new HrMasterDataClassesDataContext();
            context.CommandTimeout = 0;
            return context;
        }

        public static TimeKeeperDataClassesDataContext GetContextTimeKeeper()
        {
            return new TimeKeeperDataClassesDataContext();
        }

        public static HrSelf.EmployeeMaster GetManager(int empMasterId)
        {
            var master = GetHrMasterDb();
            var jam = master.JobAssignMasters.Where(x => x.EmpMasterID.Value == empMasterId && x.IsActive == 'Y').SingleOrDefault();
            if (jam != null && jam.DirectManager.HasValue)
            {
                var em = master.EmployeeMasters.Where(x => x.EmpMasterID == jam.DirectManager.Value).SingleOrDefault();
                return em;
            }
            return null;
        }

        public static void RecursivelyResetInputs(ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                SetReset(control);
            }
        }

        private static void SetReset(Control control)
        {
            //set the control read only
            SetControlReset(control);

            if (control.Controls != null && control.Controls.Count > 0)
            {
                //recursively loop through all child controls
                foreach (Control c in control.Controls)
                    SetReset(c);
            }
        }

        private static void SetControlReset(Control control)
        {
            var tb = control as TextBox;
            if (tb != null)
            {
                tb.Text = "";
            }

            var ddl = control as DropDownList;
            if (ddl != null)
            {
                ddl.SelectedIndex = 0;
            }

            var rb = control as RadioButton;
            if (rb != null)
            {
                rb.Checked = false;
            }

            var cb = control as CheckBox;
            if (cb != null)
            {
                cb.Checked = false;
            }
        }

        public static void RecursivelyMarkControlsReadOnly(ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                SetReadOnly(control, true);
            }
        }

        private static void SetReadOnly(Control control, bool readOnly)
        {
            //set the control read only
            SetControlReadOnly(control, readOnly);

            if (control.Controls != null && control.Controls.Count > 0)
            {
                //recursively loop through all child controls
                foreach (Control c in control.Controls)
                    SetReadOnly(c, readOnly);
            }
        }

        private static void SetControlReadOnly(Control control, bool readOnly)
        {
            var tb = control as TextBox;
            if (tb != null)
            {
                tb.ReadOnly = true;
                tb.Enabled = true;
                //tb.BorderStyle = BorderStyle.None;
                //tb.BorderWidth = 0;
                tb.Attributes.Add("style", "border: 0px none;");
                //tb.BorderColor = Color.LightGray;
            }

            var ddl = control as DropDownList;
            if (ddl != null)
            {
                ddl.Enabled = false;
                //tb.BorderStyle = BorderStyle.None;
                //tb.BorderWidth = 0;
                //tb.BorderColor = Color.LightGray;
            }

            var rb = control as RadioButton;
            if (rb != null)
            {
                //rb.Attributes.Add("onclick", "return false");
                rb.Attributes.Add("disabled", "true");
                //rb.Enabled = false;
                //tb.BorderStyle = BorderStyle.Solid;
                //tb.BorderWidth = 1;
                //tb.BorderColor = Color.LightGray;
            }

            var cb = control as CheckBox;
            if (cb != null)
            {
                //cb.Attributes.Add("onclick", "return false");
                cb.Attributes.Add("disabled", "true");
                //rb.Enabled = false;
                //tb.BorderStyle = BorderStyle.Solid;
                //tb.BorderWidth = 1;
                //tb.BorderColor = Color.LightGray;
            }

            var btn = control as Button;
            if (btn != null)
            {
                //btn.Enabled = false;
                btn.Visible = false;
            }

            var img = control as System.Web.UI.WebControls.Image;
            if (img != null)
            {
                if (img.ImageUrl.Contains("calendar_icon.gif"))
                {
                    img.Visible = false;
                }
            }
        }

        static public string TimeOffColor(string InternalName)
        {
            switch (InternalName)
            {
                case "Bereavement":
                    return "#a64b00";
                case "Birth":
                    return "#00cc00";
                case "Business Travel":
                    return "#1d7373";
                case "Floating Holiday":
                    return "#008500";
                case "Jury Duty":
                    return "#ff9640";
                case "Marriage":
                    return "#006363";
                //return "#269926";	// was "Wedding"
                case "Military Duty":
                    return "#a60000";
                case "Personal":
                    return "#009999";
                case "Relocation":
                    return "#ffb273";
                case "Sick":
                    return "#5ccccc";
                case "Vacation":
                    return "#00CC00";
            }
            return "#67e667";
        }
    }
}