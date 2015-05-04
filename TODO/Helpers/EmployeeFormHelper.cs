using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Core.Helpers
{
    public class EmployeeFormHelper
    {
        public static KeyValuePair<string, string>[] Employees
        {
            get
            {
                using (var Master = DBHelper.GetMasterDB())
                {
                    var Employees = Master.usp_AllEmployees(null, null, DBHelper.GetEntityMasterID(), false);

                    return Employees.Select(x => new KeyValuePair<string, string>(x.EmpMasterID.ToString(), x.EmployeeFullName)).ToArray();
                }
            }
        }

        public static KeyValuePair<string, string>[] Entities
        {
            get
            {
                using (var Master = DBHelper.GetMasterDB())
                {
                    var Entities = Master.EntityMasters
                        .Where(x => x.IsActive == 'Y' && x.EntityType == 1)
                        .Select(x => new KeyValuePair<string, string>(x.EntityMasterID.ToString(), x.EntityName)).ToArray();

                    return Entities;
                }
            }
        }

        public static KeyValuePair<string, string>[] EntitiesExceptCurrent
        {
            get
            {
                using (var Master = DBHelper.GetMasterDB())
                {
                    var CurrentEntityMasterID = DBHelper.GetEntityMasterID();
                    var Entities = Master.EntityMasters
                        .Where(x => x.EntityMasterID != CurrentEntityMasterID && x.IsActive == 'Y' && x.EntityType == 1)
                        .Select(x => new KeyValuePair<string, string>(x.EntityMasterID.ToString(), x.EntityName)).ToArray();

                    return Entities;
                }
            }
        }

        public static KeyValuePair<string, string>[] Locations
        {
            get
            {
                using (var Master = DBHelper.GetMasterDB())
                {
                    return Master.OfficeLocations.Select(x => new KeyValuePair<string, string>(x.OfficeLocationID.ToString(), x.ShortName)).ToArray();
                }
            }
        }

        public static KeyValuePair<string, string>[] AllEmployeesByADUserID
        {
            get
            {
                using (var Master = DBHelper.GetMasterDB())
                {
                    var Employees = Master.usp_AllEmployees(null, null, null, false);

                    return Employees.Select(x => new KeyValuePair<string, string>(x.ADDomain + "\\" + x.ADUserId, x.EmployeeFullName)).ToArray();
                }
            }
        }
    }
}
