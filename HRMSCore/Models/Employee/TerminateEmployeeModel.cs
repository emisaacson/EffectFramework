using Local.Classes.Attributes;
using Local.Classes.Helpers;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Local.Models.Employee
{
    public class TerminateEmployeeModel
    {
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Source Entity")]
        [Choices(typeof(EmployeeFormHelper), "Entities")]
        [Required]
        public int? SourceEntity { get; set; }

        [Required]
        [DisplayName("Employee ID Leaving the Company")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices]//[Choices(typeof(EmployeeFormHelper), "Employees")]
        public int? EmpMasterID { get; set; }

        [DisplayName("Sensitive Termination (only IT notified)")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public bool? SensitiveTermination { get; set; }

        [DisplayName("Equipment Location")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices(typeof(EmployeeFormHelper), "Locations")]
        public int? EquipmentLocation { get; set; }

        [Required]
        [DisplayName("Work Location")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices(typeof(EmployeeFormHelper), "Locations")]
        public int? WorkLocation { get; set; }

        [Required]
        [DisplayName("Termination Type")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices("Involuntary", "Record Change", "Voluntary")]
        public string TerminationType { get; set; }

        [Required]
        [DisplayName("Termination Reason")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices]
        public string TerminationReason { get; set; }

        [Required]
        [DisplayName("Loss to the Company")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices("Yes", "No")]
        public string IsLoss { get; set; }

        [DisplayName("Grant mailbox permission to")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices]
        public string GrantMBPermissionTo { get; set; }

        [DisplayName("Forward new email to")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices]
        public string ForwardNewEmailTo { get; set; }

        [Required]
        [DisplayName("Termination Date")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public DateTime? TerminationDate { get; set; }

        [Required]
        [DisplayName("Pay Through Date")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public DateTime? PayThruDate { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DefaultValue(true)]
        [DisplayName("Disable Immediately")]
        public bool? DisableImmediately { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("When to Disable Account")]
        public DateTime? WhenToDisableAccount { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Textarea]
        [DisplayName("Additional Comments")]
        public string AdditionalComments { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public string ForwardNewEmailToEmployeeFullName {
            get
            {
                if (ForwardNewEmailTo != null)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var EmpMaster = Master.EmployeeMasters
                            .Where(x => ((x.ADDomain ?? "")+"\\"+(x.ADUserID ?? "")).ToLower() == ForwardNewEmailTo.ToLower() && x.IsActive == 'Y')
                            .OrderBy(x => x.EmpStatus == "Active" ? 0 : 1)
                            .ThenByDescending(x => x.EmpMasterID)
                            .SingleOrDefault();

                        if (EmpMaster != null)
                        {
                            return EmpMaster.EmployeeReverseFullName;
                        }

                    }
                }

                return null;
            }
        }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public string GrantMBPermissionToEmployeeFullName
        {
            get
            {
                if (GrantMBPermissionTo != null)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var EmpMaster = Master.EmployeeMasters
                            .Where(x => ((x.ADDomain ?? "") + "\\" + (x.ADUserID ?? "")).ToLower() == GrantMBPermissionTo.ToLower() && x.IsActive == 'Y')
                            .OrderBy(x => x.EmpStatus == "Active" ? 0 : 1)
                            .ThenByDescending(x => x.EmpMasterID)
                            .SingleOrDefault();

                        if (EmpMaster != null)
                        {
                            return EmpMaster.EmployeeReverseFullName;
                        }

                    }
                }

                return null;
            }
        }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Employee Full Name Leaving the Company")]
        public string EmployeeFullName
        {
            get
            {
                if (EmpMasterID != null && EmpMasterID.HasValue)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var EmpMaster = Master.EmployeeMasters.Where(x => x.EmpMasterID == EmpMasterID && x.IsActive == 'Y').SingleOrDefault();
                        if (EmpMaster != null)
                        {
                            return EmpMaster.EmployeeFullName;
                        }
                        return null;
                    }
                }

                return null;
            }
        }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public string EmployeeReverseFullName
        {
            get
            {
                if (EmpMasterID != null && EmpMasterID.HasValue)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var EmpMaster = Master.EmployeeMasters.Where(x => x.EmpMasterID == EmpMasterID && x.IsActive == 'Y').SingleOrDefault();
                        if (EmpMaster != null)
                        {
                            return EmpMaster.EmployeeReverseFullName;
                        }
                        return null;
                    }
                }

                return null;
            }
        }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public string EmployeeADName
        {
            get
            {
                if (EmpMasterID != null && EmpMasterID.HasValue)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var EmpMaster = Master.EmployeeMasters.Where(x => x.EmpMasterID == EmpMasterID && x.IsActive == 'Y').SingleOrDefault();
                        if (EmpMaster != null)
                        {
                            return EmpMaster.ADUserID;
                        }
                    }
                }
                return null;
            }
        }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public string SourceEntityName
        {
            get
            {
                using (var Master = DBHelper.GetMasterDB())
                {
                    var Entity = Master.EntityMasters.Where(en => en.EntityMasterID == SourceEntity).FirstOrDefault();

                    return Entity != null ? Entity.EntityName : null;
                }
            }
        }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public string WorkLocationText
        {
            get
            {
                if (this.WorkLocation.HasValue)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var location = Master.OfficeLocations.Where(x => x.OfficeLocationID == this.WorkLocation).FirstOrDefault();

                        if (location != null)
                        {
                            return location.ShortName;
                        }
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public string EquipmentLocationText
        {
            get
            {
                if (this.EquipmentLocation.HasValue)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var location = Master.OfficeLocations.Where(x => x.OfficeLocationID == this.EquipmentLocation).FirstOrDefault();

                        if (location != null)
                        {
                            return location.ShortName;
                        }
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}