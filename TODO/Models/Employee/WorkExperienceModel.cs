using HRMS.Modules.DBModel.Local;
using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.Employee
{
    [DisplayName("Work Experience")]
    [LocalTable("EmpWorkExps", typeof(EmpWorkExp))]
    [EmpGeneralIDProperty("EmpGenID")]
    public class WorkExperienceModel : Model
    {
        [LocalProperty("EmpWorkExpID")]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Work Experience ID")]
        [Key]
        public int? WorkExperienceID { get; set; }

        [LocalProperty("WorkExpType")]
        [Choices("Consultant", "Contractor", "Co-op", "Full-Time Employee", "Internship", "Other", "Part-Time Employee")]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Type")]
        [MaxLength(50)]
        [Required]
        public string ExperienceType { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Employer")]
        [MaxLength(50)]
        [Required]
        public string EmployerName { get; set; }

        [LocalProperty("EmployerIndustry")]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [MaxLength(50)]
        [Choices("Communications", "Financial", "Manufacturing", "Other", "Technology")]
        public string Industry { get; set; }

        [LocalProperty("EmployerMainPhone")]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Employer Phone")]
        [MaxLength(50)]
        public string Phone { get; set; }

        [LocalProperty("EmployerAddress1")]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Employer Address1")]
        [MaxLength(50)]
        public string Address1 { get; set; }

        [LocalProperty("EmployerAddress2")]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Employer Address2")]
        [MaxLength(50)]
        public string Address2 { get; set; }

        [LocalProperty("EmployerCity")]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Employer City")]
        [MaxLength(50)]
        public string City { get; set; }

        [LocalProperty("EmployerState")]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Employer State")]
        [MaxLength(50)]
        public string State { get; set; }

        [LocalProperty("EmployerPostalCode")]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Employer Postal Code")]
        [MaxLength(50)]
        public string PostalCode { get; set; }

        [LocalProperty("EmployerCountry")]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Employer Country")]
        [MaxLength(50)]
        public string Country { get; set; }

        [LocalProperty("EmployerWebsite")]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Employer Website")]
        [MaxLength(50)]
        public string Website { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Position Title")]
        [MaxLength(50)]
        public string PositionTitle { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Position Level")]
        [MaxLength(50)]
        [Choices("Board Member", "Chairman", "Director", "Executive", "Manager", "Other", "President", "Staff Level", "Sr. Manager", "Supervisor", "Vice President")]
        public string PositionLevel { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Position Description")]
        public string PositionDesc { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Manager's Name")]
        [MaxLength(50)]
        public string ManagersName { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Reason for Leaving")]
        [MaxLength(50)]
        [Choices("Employee Decision", "Other")]
        public string ReasonForLeaving { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Notes")]
        [MaxLength(50)]
        [Textarea]
        public string ReasonForLeavingDesc { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Position Start Date")]
        public DateTime? PositionStartDate { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [DisplayName("Position End Date")]
        public DateTime? PositionEndDate { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [CurrencyField]
        public decimal? Salary { get; set; }

        [LocalProperty("SalaryCur")]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [LocalReference("Currencies", typeof(Currency), "CurID", "CurName", new string[] { "IsActive", "Y" })]
        [DisplayName("Salary Currency")]
        public string SalaryCurrency { get; set; }

        [LocalProperty("SalaryFeq")]
        //[RequirePermission(Permission._EMP_WRK_EXP)]
        [Choices("Per Hour", "Per Day", "Per Month", "Per Week", "Per Year")]
        [DisplayName("Salary Frequency")]
        public string SalaryFrequency { get; set; }
    }
}
