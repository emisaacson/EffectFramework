using HRMS.Core.Attributes;
using HRMS.Core.Helpers;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.Employee
{
    public class EmployeeTransferModel
    {
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Source Entity")]
        [Choices(typeof(EmployeeFormHelper), "Entities")]
        [Required]
        public int? SourceEntity { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Source Employee")]
        [Choices]//[Choices(typeof(EmployeeFormHelper), "Employees")]
        [Required]
        public int? SourceEmpMasterID { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Destination Entity")]
        [Choices]//[Choices(typeof(EmployeeFormHelper), "EntitiesExceptCurrent")]
        [Required]
        public int? DestinationEntity { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices]
        [Required]
        [DisplayName("Destination Company")]
        public int? Company { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices]
        [Required]
        [DisplayName("Direct Manager")]
        public int? DirectManager { get; set; }

        [Required]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Job Title")]
        public string JobTitle { get; set; }

        [DisplayName("Global Business Unit")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices]
        [Required]
        public int? GBU { get; set; }

        [DisplayName("Cost Center Code")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices]
        [Required]
        public int? CostCenter { get; set; }

        [DisplayName("Global Department Code")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices]
        [Required]
        public int? DeptCode { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices(typeof(EmployeeFormHelper), "Locations")]
        [Required]
        public int? Location { get; set; }

        [Required]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [Required]
        [MasterProperty]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DisplayName("Pay Through Date")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public DateTime? PayThruDate { get; set; }

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
        [DisplayName("Destination Entity")]
        public string DestinationEntityName
        {
            get
            {
                using (var Master = DBHelper.GetMasterDB())
                {
                    var Entity = Master.EntityMasters.Where(en => en.EntityMasterID == DestinationEntity).FirstOrDefault();

                    return Entity != null ? Entity.EntityName : null;
                }
            }
        }

        [DisplayName("Destination Company")]
        public string CompanyText
        {
            get
            {
                if (this.Company.HasValue)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var Company = Master.CompanyMasters.Where(x => x.CompMasterID == this.Company).FirstOrDefault();

                        if (Company != null)
                        {
                            return Company.CompanyName;
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

        [DisplayName("Direct Manager")]
        public string DirectManagerName
        {
            get
            {
                if(this.DirectManager.HasValue)
                {
                    using(var Master = DBHelper.GetMasterDB())
                    {
                        var Manager = Master.EmployeeMasters.FirstOrDefault(x => x.EmpMasterID == DirectManager);
                        if(Manager != null)
                        {
                            return Manager.EmployeeFullName;
                        }
                    }
                }
                return String.Empty;
            }
        }

        [DisplayName("Global Business Unit")]
        public string GBUText
        {
            get
            {
                if (this.GBU.HasValue)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var GBU = Master.GBUCorpFuncs.Where(x => x.GBUFuncID == this.GBU).FirstOrDefault();

                        if (GBU != null)
                        {
                            return GBU.GBUFuncName;
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

        [DisplayName("Cost Center Code")]
        public string CCText
        {
            get
            {
                if (this.CostCenter.HasValue)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var CC = Master.FinanceCostCodes.Where(x => x.FinCCID == this.CostCenter).FirstOrDefault();

                        if (CC != null)
                        {
                            return CC.FinCCode;
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

        [DisplayName("Global Department Code")]
        public string DCText
        {
            get
            {
                if (this.DeptCode.HasValue)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var DC = Master.GlobalDeptCodes.Where(x => x.GlobalDCID == this.DeptCode).FirstOrDefault();

                        if (DC != null)
                        {
                            return DC.GlobalDCName;
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

        public string LocationText
        {
            get
            {
                if (this.Location.HasValue)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var location = Master.OfficeLocations.Where(x => x.OfficeLocationID == this.Location).FirstOrDefault();

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

        [DisplayName("Source Employee Full Name")]
        public string EmployeeFullName
        {
            get
            {
                if (SourceEmpMasterID != null && SourceEmpMasterID.HasValue)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var EmpMaster = Master.EmployeeMasters.Where(x => x.EmpMasterID == SourceEmpMasterID && x.IsActive == 'Y').SingleOrDefault();
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
    }
}
