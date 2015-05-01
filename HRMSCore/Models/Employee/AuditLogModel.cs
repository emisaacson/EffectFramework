using Local.Classes.Attributes;
using Local.Classes.Security;
using HRMS.Modules.DBModel.Local;
using HRMS.Modules.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Local.Models.Employee
{
    [MasterTable("MasterAuditLogs", typeof(MasterAuditLog))]
    [LocalTable("LocalAuditLogs", typeof(LocalAuditLog))]
    public class AuditLogModel : Model
    {
        [MasterProperty]
        [LocalProperty]
        //[RequirePermission(Permission._SYSTEM_LOGS)]
        public string Table { get; set; }

        [MasterProperty]
        [LocalProperty]
        //[RequirePermission(Permission._SYSTEM_LOGS)]
        public string Field { get; set; }

        [MasterProperty]
        [LocalProperty]
        //[RequirePermission(Permission._SYSTEM_LOGS)]
        public int EmpMasterID { get; set; }

        [MasterProperty]
        [LocalProperty]
        //[RequirePermission(Permission._SYSTEM_LOGS)]
        public string OldValue { get; set; }

        [MasterProperty]
        [LocalProperty]
        //[RequirePermission(Permission._SYSTEM_LOGS)]
        public string NewValue { get; set; }

        [MasterProperty]
        [LocalProperty]
        //[RequirePermission(Permission._SYSTEM_LOGS)]
        public DateTime UpdateDate { get; set; }

        [MasterProperty]
        [LocalProperty]
        //[RequirePermission(Permission._SYSTEM_LOGS)]
        public string RealUser { get; set; }

        [MasterProperty]
        [LocalProperty]
        //[RequirePermission(Permission._SYSTEM_LOGS)]
        public string ImpersonatingUser { get; set; }

        [MasterProperty]
        [LocalProperty]
        //[RequirePermission(Permission._SYSTEM_LOGS)]
        public bool IsSelfService { get; set; }

        [MasterProperty]
        [LocalProperty]
        //[RequirePermission(Permission._SYSTEM_LOGS)]
        public int ItemID { get; set; }

        [MasterProperty]
        [LocalProperty]
        //[RequirePermission(Permission._SYSTEM_LOGS)]
        public string Comment { get; set; }
    }
}
