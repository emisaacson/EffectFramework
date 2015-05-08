using HRMS.Modules.DBModel;
using HRMS.Core.Attributes;
using HRMS.Core.Helpers;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using Humanizer;

namespace HRMS.Core.Models.Employee
{
    [DisplayName("Org Alignment")]
    [MasterTable("JobAssignMasters", typeof(JobAssignMaster))]
    public class JobAssignMasterModel : Model
    {
        [MasterProperty]
        [DisplayName("Job Assignment ID")]
        //[RequirePermission(Permission._EMP_JOB)]
        [Key]
        public int? JobAssignID { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Employee Master ID")]
        [Required]
        public int? EmpMasterID { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Source Employee General ID")]
        [Required]
        [HideInCustomReports]
        public int? SourceEmpGeneralID { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Source Database Name")]
        [Choices(typeof(Entities), "ALL_ENTITIES_WITH_BRACKETS")]
        [Required]
        public string SourceDatabaseName { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Job Title")]
        [Required]
        public string JobTitle { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Company")]
        [Choices]
        [Required]
        public int? CompanyID { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Direct Manager")]
        [Choices]
        [HideInCustomReports]
        public int? DirectManager { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Probation Start")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [EntityRestriction(Entities._3iMind, Entities._4D, Entities.AGTBrazil, Entities.AGTChina, Entities.AGTGermany, Entities.AGTIndia, Entities.AGTNetherlands,
                           Entities.AGTSingapore, Entities.AGTSwitzerland, Entities.AGTUK, Entities.ATSD, Entities.Logic)]
        public DateTime? ProbationStart { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Probation End")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [EntityRestriction(Entities._3iMind, Entities._4D, Entities.AGTBrazil, Entities.AGTChina, Entities.AGTGermany, Entities.AGTIndia, Entities.AGTNetherlands,
                           Entities.AGTSingapore, Entities.AGTSwitzerland, Entities.AGTUK, Entities.ATSD, Entities.Logic)]
        public DateTime? ProbationEnd { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Job End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? JobEndDate { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Job Level")] // Not a typo :-/
        [Choices("Technical-Team Lead", "President", "Vice President", "Individual", "Manager", "Director", "Senior Manager", "Professional", "Global Executive")]
        [Required]
        public string JobType { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [Choices("Y|Yes", "N|No")]
        [HideInCustomReports]
        public char? IsActive { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [Choices("Y|Yes", "N|No")]
        [HideInCustomReports]
        public char IsDeleted { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Entity Code")]
        [MasterReference("GlobalEntityCodes", typeof(HRMS.Modules.DBModel.GlobalEntityCode), "GlobalECID", "ECName", new string[] { "IsActive", "Y" })]
        public int? GlobalECID { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("GBU")]
        [Required]
        [Choices]
        public int? GBUFuncID { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Cost Code")]
        [Required]
        [Choices]
        public int? FinCCID { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Department Code")]
        [Required]
        [Choices]
        public int? GlobalDCID { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Work Percent")]
        [Range(0, 100)]
        [PercentageField]
        [DefaultValue(100)]
        public decimal? WorkPct { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Job Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        public DateTime? JobStartDate { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Create Date")]
        public DateTime CreateDate { get; set; }

        [MasterProperty("DivisionID")]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Division")]
        // ATS is EntityMasterID 9 -- these two below need to be reworked if the Entity restriction is ever changed
        [EntityRestriction(Entities.ATSD)]
        [MasterReference("DivisionMasters", typeof(DivisionMaster), "DivMasterID", "DivName", new string[] { "IsActive", "Y", "EntityMasterID", "9" })]
        public int? Division { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Change Reason")]
        public string ChangeReason { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Direct/Indirect")]
        [Choices("Direct", "Indirect")]
        [Required]
        public string DirectIndirect { get; set; }

        [MasterProperty("EmpType")]
        [MasterReference("EmployeeTypes", typeof(HRMS.Modules.DBModel.EmployeeType), "EmpTypeID", "TypeName", new string[] { "IsActive", "Y" })]
        //[RequirePermission(Permission._EMP_BASIC)]
        [DisplayName("Employee Type")]
        [DefaultValue(0)]
        public int? EmployeeType { get; set; }

        [MasterProperty]
        [DefaultValue(false)]
        [EntityRestriction(Entities.Star, Entities.AGTSwitzerland, Entities.AGTGermany, Entities.Vocativ, Entities.ATSD, Entities.AGTSingapore)]
        [DisplayName("Pro Rate Hours ETAS")]
        [Choices("true|Yes", "false|No")]
        public bool? ProRateHours { get; set; }


        public override void Validate()
        {

            List<ValidationResult> Errors = new List<ValidationResult>();

            // Only validate new or active jobs
            if ((this.IsActive.HasValue && this.IsActive.Value == 'Y') || !this.JobAssignID.HasValue)
            {
                var unlockedFields = this.GetType().GetProperties().Where(x => DBHelper.GetCurrentUserContext().HasPermissionTo(x, PermissionLevel.CREATE));
                foreach (var item in unlockedFields)
                {
                    Validator.TryValidateProperty(item.GetValue(this),
                        new ValidationContext(this, null, null) { MemberName = item.Name },
                        Errors);
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