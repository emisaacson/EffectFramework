using HRMS.Modules.DBModel.Local;
using Local.Classes.Attributes;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Local.Models.Employee
{
    [DisplayName("Dependents")]
    [LocalTable("EmpDependents", typeof(EmpDependent))]
    [EmpGeneralIDProperty("EmpID")]
    public class DependentModel : Model
    {
        [LocalProperty("DepID")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [DisplayName("Dependent ID")]
        [Key]
        //[SelfService]
        public int? DependentID { get; set; }

        [LocalProperty("DepType")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [DisplayName("Dependent Type")]
        [LocalReference("EmpDependentTypes", typeof(EmpDependentType), "DepTypeID", "DepTypeName", new string[] { "IsActive", "Y" })]
        [Required]
        //[SelfService]
        public int DependentType { get; set; }

        [LocalProperty("DOB")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [DisplayName("Birth Date")]
        //[SelfService]
        public DateTime? DateOfBirth { get; set; }

        [LocalProperty("Gender")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [Choices("M|Male", "F|Female")]
        [DisplayName("Gender")]
        //[SelfService]
        public char? Gender { get; set; }

        [LocalProperty("FirstName")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [DisplayName("First Name")]
        [Required]
        //[SelfService]
        public string FirstName { get; set; }

        [LocalProperty("MiddleName")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [DisplayName("Middle Name")]
        //[SelfService]
        public string MiddleName { get; set; }

        [LocalProperty("LastName")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [DisplayName("Last Name")]
        [Required]
        //[SelfService]
        public string LastName { get; set; }

        [LocalProperty("CitizenID")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [DisplayName("Citizen ID")]
        //[Encrypted]
        //[SelfService]
        public string CitizenID { get; set; }

        [LocalProperty("LivesInCountry")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [Choices(typeof(Local.Classes.Helpers.Countries), "ALL_COUNTRIES")]
        [DisplayName("Residence")]
        //[SelfService]
        public string Residence { get; set; }

        [LocalProperty("MarriageDate")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [DisplayName("Marriage Date")]
        //[SelfService]
        public DateTime? MarriageDate { get; set; }

        [LocalProperty("OnHealthPlan")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [Choices("Y|Yes", "N|No")]
        [DisplayName("On Health Plan")]
        //[SelfService]
        public char? OnHealthPlan { get; set; }

        [LocalProperty("OnTuitionPlan")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [Choices("Y|Yes", "N|No")]
        [DisplayName("On Tuition Plan")]
        //[SelfService]
        public char? OnTuitionPlan { get; set; }

        [LocalProperty("IsBeneficiary")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [Choices("Y|Yes", "N|No")]
        [DisplayName("Beneficiary")]
        [Required]
        //[SelfService]
        public char? IsBeneficiary { get; set; }

        [LocalProperty("BeneficiaryPct")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [DisplayName("Beneficiary Percentage")]
        [MaxLength(50)]
        //[SelfService]
        public string BeneficiaryPercentage { get; set; }
    }
}
