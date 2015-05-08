using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRMS.Modules.DBModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Reflection;
using Humanizer;
using HRMS.Core.Helpers;

namespace HRMS.Core.Models.Employee
{
    public class FinanceCostCodeModel
    {
        public int? FinCCID { get; set; }
        public string FinCCName { get; set; }
        public string FinCCDesc { get; set; }
        public string ExpenseGroupName { get; set; }
        public int? ExpenseGroupID { get; set; }
        public bool IsActive { get; set; }
    }
    public class GBUModel
    {
        public int? GBUFuncID { get; set; }
        public string GBUFuncName { get; set; }
        public bool IsActive { get; set; }
    }
    public class ManagerModel
    {
        public int? EmpMasterID { get; set; }
        public string ManagerName { get; set; }
        public int? ManagerManagerEmpMasterID { get; set; }
        public bool IsActive { get; set; }
        public bool OKInCurrentEntity { get; set; }
        public int[] Entities { get; set; }
    }
    public class CompanyModel
    {
        public int? CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string EntityDatabase { get; set; }
        public bool IsActive { get; set; }
    }
    public class JobAssignmentsCollection : Model
    {
        //[RequirePermission(Permission._EMP_JOB)]
        public IEnumerable<JobAssignMasterModel> Items { get; set; }

        public IEnumerable<CompanyModel> CompanyLookup { get; set; }
        public IEnumerable<ManagerModel> ManagerLookup { get; set; }
        public IEnumerable<GBUModel> GBULookup { get; set; }
        public IEnumerable<GlobalDeptCode> DepartmentCodeLookup { get; set; }
        public IEnumerable<FinanceCostCodeModel> CostCodeLookup { get; set; }
        public IEnumerable<GBUtoFCCAssign> GBUtoFCCMapping { get; set; }


        public override void Validate()
        {

            List<ValidationResult> Errors = new List<ValidationResult>();

            int cnt = 1;
            foreach (JobAssignMasterModel Item in Items)
            {
                // Only validate new or active jobs
                if ((Item.IsActive.HasValue && Item.IsActive.Value == 'Y') || !Item.JobAssignID.HasValue)
                {
                    var unlockedFields = Item.GetType().GetProperties().Where(x => DBHelper.GetCurrentUserContext().HasPermissionTo(x, PermissionLevel.CREATE));
                    foreach (var item in unlockedFields)
                    {
                        string DisplayName = item.Name;
                        if (Attribute.IsDefined(item, typeof(DisplayNameAttribute)))
                        {
                            var DNAttribute = item.GetCustomAttribute<DisplayNameAttribute>();
                            DisplayName = DNAttribute.DisplayName;
                        }
                        Validator.TryValidateProperty(item.GetValue(Item),
                            new ValidationContext(Item, null, null) { MemberName = item.Name, DisplayName = DisplayName + " (" + cnt.ToOrdinalWords() + " item)", },
                            Errors);
                    }
                    cnt++;
                }
            }

            HRValidationResult Result = new HRValidationResult()
            {
                IsValid = Errors.Count() == 0,
                RawErrors = Errors,
            };

            _Validation = Result;
        }
    }
}