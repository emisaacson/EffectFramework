using HRMS.Modules.DBModel;
using HRMS.Modules.DBModel.Local;
using HRMS.Core.Attributes;
using HRMS.Core.Helpers;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace HRMS.Core.Models.Employee
{
	[DisplayName("Employee Basic Information")]
	[LocalTable("EmpGenerals", typeof(EmpGeneral))]
	[MasterTable("EmployeeMasters", typeof(EmployeeMaster))]
	[DataContract]
	public class EmployeeModel : Model
	{
		[MasterProperty]
		[DisplayName("Employee ID")]
		//[RequirePermission(Permission._EMP_BASIC)]
		//[ReadOnly(true)]
		[DataMember]
        //[SelfService]
		public int EmpMasterID { get; set; }

		[LocalProperty]
		[DisplayName("Local Employee ID")]
		[MasterProperty("SourceEmpGeneralId")]
		//[RequirePermission(Permission._EMP_BASIC)]
		//[ReadOnly(true)]
		[DataMember]
        [HideInCustomReports]
		public int EmpGeneralID { get; set; }

		[LocalProperty]
		[Choices("Mr.", "Mrs.", "Miss", "Ms.", "Dr.", "Prof.")]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DataMember]
        //[SelfService]
		public string Salutation { get; set; }

        private string fullName;
		[DisplayName("Full Name")]
        //[ReadOnly(true)]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DataMember]
		public string FullName
		{
			get
			{
				return (string.IsNullOrWhiteSpace(FirstNameAlias) ? FirstName : FirstNameAlias) + " " +
					   (string.IsNullOrWhiteSpace(LastNameAlias) ? LastName : LastNameAlias);
			}
            set
            {
                fullName = value;
            }
		}

		[LocalProperty]
		[MasterProperty]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DisplayName("First Name")]
		[Required]
		[DataMember]
        //[SelfService(true)]
		public string FirstName { get; set; }

		[LocalProperty]
		[MasterProperty]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DisplayName("Middle Name")]
		[DataMember]
        //[SelfService(true)]
		public string MiddleName { get; set; }

		[LocalProperty]
		[MasterProperty]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DisplayName("Last Name")]
		[Required]
		[DataMember]
        //[SelfService(true)]
		public string LastName { get; set; }

		[LocalProperty("FNAlias")]
		[MasterProperty("FNAlias")]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DisplayName("First Name Alias")]
		[DataMember]
        //[SelfService(true)]
		public string FirstNameAlias { get; set; }

		[LocalProperty("LNAlias")]
		[MasterProperty("LNAlias")]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DisplayName("Last Name Alias")]
		[DataMember]
        //[SelfService(true)]
		public string LastNameAlias { get; set; }

		[MasterProperty]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DataMember]
        //[SelfService]
		public string Nickname { get; set; }

		[LocalProperty("ADDomain")]
		[MasterProperty("ADDomain")]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DataMember]
		//[ReadOnly(true)]
		public string Domain { get; set; }

		[LocalProperty("ADUserID")]
		[MasterProperty("ADUserID")]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DataMember]
		//[ReadOnly(true)]
		public string Username { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_BASIC)]
        [DataMember]
        [Textarea]
        public string Remarks { get; set; }

        private string userAccount;
		[DisplayName("User Account")]
		//[ReadOnly(true)]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DataMember]
		public string UserAccount
		{
			get
			{
				if (Domain != null && Username != null)
				{
					return Domain.ToUpper() + "\\" + Username.ToLower();
				}
				else
				{
					return null;
				}
			}
            set
            {
                userAccount = value;
            }
		}

		[LocalProperty]
		//[RequirePermission(Permission._EMP_BASIC)]
		[Choices("M|Male", "F|Female")]
		[DataMember]
		[Required]
        //[SelfService]
		public char? Gender { get; set; }

		[LocalProperty]
		//[RequirePermission(Permission._EMP_BASIC)]
		public System.Data.Linq.Binary Picture { get; set; }

		//[RequirePermission(Permission._EMP_BASIC)]
		[UIHint("Photo")]
		[DataMember]
        //[SelfService]
		public string Photo
		{
			get
			{
				if (this.Picture == null)
					return null;

				string photo = Convert.ToBase64String(this.Picture.ToArray());
				return string.Format("data:image/jpeg;base64,{0}", photo);
			}
			set { }
		}

		[LocalProperty("DOB")]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
		[DisplayName("Birth Date")]
        //[SelfService]
		[DataMember]
		[Required]
		public DateTime? DateOfBirth { get; set; }

		[LocalProperty]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DisplayName("Tax ID")]
		[DataMember]
		[EntityRestriction(Entities.AGTGermany)]
		[MaxLength(1024)]
		public string TaxID { get; set; }

		[LocalProperty("EmpStatus")]
		[MasterProperty("EmpStatus")]
		//[RequirePermission(Permission._EMP_BASIC)]
		[Choices("Active", "Terminated")]
		//[ReadOnly(true)]
        [HideInCustomReports]
		[DataMember]
		public string Status { get; set; }

        [MasterProperty("EmpDerivedStatus")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [DisplayName("Employee Status")]
        [Choices("Active", "Terminated", "Pending Transfer", "Pending Termination", "Pending Hire")]
        //[ReadOnly(true)]
        [DataMember]
        public string EmployeeDerivedStatus { get; set; }

        [MasterProperty("EmpStatusType")]
        [DisplayName("Employee Status Type")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [Choices("Active", "Inactive")]
        //[ReadOnly(true)]
        [DataMember]
        public string EmploymentStatusType { get; set; }

		[LocalProperty]
		//[RequirePermission(Permission._EMP_BASIC)]
		[Choices(typeof(HRMS.Core.Helpers.Countries), "ALL_COUNTRIES")]
		[DataMember]
        //[SelfService]
		public string Citizenship { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_BASIC)]
        [Choices(typeof(HRMS.Core.Helpers.Countries), "ALL_COUNTRIES")]
        [DataMember]
        [DisplayName("Second Citizenship")]
        //[SelfService]
        public string SecondCitizenship { get; set; }

		[LocalProperty("CitizenID")]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DisplayName("Citizenship ID")]
		[DataMember]
        [FieldDescription("Tax ID number used in the country in which you are employed.")]
        //[SelfService]
        //[Encrypted]
		public string CitizenshipID { get; set; }

        [LocalProperty("SecondCitizenID")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [DisplayName("Second Citizenship ID")]
        [DataMember]
        [FieldDescription("Tax ID number used in the country in which you are employed.")]
        //[SelfService]
        //[Encrypted]
        public string SecondCitizenshipID { get; set; }

		[LocalProperty]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DisplayName("Birth Country")]
		[Choices(typeof(HRMS.Core.Helpers.Countries), "ALL_COUNTRIES")]
		[DataMember]
        //[SelfService]
		[Required]
		public string BirthCountry { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._SECURITY_BRIEFING)]
        [EntityRestriction(Entities.AGTSwitzerland, Entities._4D, Entities.AGTBrazil,
            Entities.AGTChina, Entities.AGTGermany, Entities.AGTIndia, Entities.AGTNetherlands,
            Entities.AGTSingapore, Entities.AGTUK, Entities.ATSD, Entities.Star)]
        [DisplayName("Completed Security Briefing")]
        [Choices("true|Yes", "false|No")]
        [DefaultValue(true)]
        [DataMember]
        public bool? CompletedSecurityBriefing { get; set; }

		[LocalProperty]
		[Choices("M|Married", "S|Single", "W|Widowed", "D|Divorced")]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DisplayName("Marital Status")]
        //[SelfService]
		[DataMember]
		public char? MaritalStatus { get; set; }

		[LocalProperty]
		[Choices("Y|Yes", "N|No")]
		//[RequirePermission(Permission._EMP_BASIC)]
		[EntityRestriction(Entities.AGTSwitzerland)]
		[DataMember]
		public char? Disability { get; set; }

		[MasterProperty("EmpType")]
		[LocalProperty("EmpType")]
		[MasterReference("EmployeeTypes", typeof(HRMS.Modules.DBModel.EmployeeType), "EmpTypeID", "TypeName", new string[] { "IsActive", "Y" })]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DisplayName("Employee Type")]
		[DataMember]
		[Required]
		public int? EmployeeType { get; set; }

		[LocalProperty("NewOrRehire")]
		[Choices("N|New", "R|Rehire","T|Transfer")]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DisplayName("Hire Status")]
		[DataMember]
		[Required]
		public char? NewOrRehire { get; set; }

		[LocalProperty("DateOfHire")]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DisplayName("Hire Date")]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
		[DataMember]
        //[SelfService(true)]
		[Required]
		public DateTime? HireDate { get; set; }

		[LocalProperty("i9")]
		//[RequirePermission(Permission._EMP_BASIC)]
		[Choices("Y|Yes", "N|No")]
		[DataMember]
		[EntityRestriction(Entities.Vocativ, Entities.Star)]
		public char? I9 { get; set; }

		[LocalProperty("i9Date")]
		//[RequirePermission(Permission._EMP_BASIC)]
		[DisplayName("I9 Date")]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
		[DataMember]
		[EntityRestriction(Entities.Vocativ, Entities.Star)]
		public DateTime? I9Date { get; set; }

		[LocalProperty]
		//[RequirePermission(Permission._EMP_BASIC)]
		[Choices("Y|Yes", "N|No")]
		[DisplayName("NDA Signed")]
		[DataMember]
		public char? NDASigned { get; set; }

		//[RequirePermission(Permission._EMP_BASIC)]
		[DataMember]
		[DisplayName("AHV Number")]
		[LocalProperty]
		[MaxLength(100)]
		[EntityRestriction(Entities.AGTSwitzerland)]
		public string AHVNo { get; set; }

		//[RequirePermission(Permission._EMP_BASIC)]
		[DataMember]
		[DisplayName("Uses Company Health")]
		[LocalProperty("UsesCompanyHealthInsurance")]
		[Choices("true|Yes", "false|No")]
		[EntityRestriction(Entities.AGTSwitzerland, Entities.Star, Entities.Vocativ)]
		public bool? UsesCompanyHealth { get; set; }

		//[RequirePermission(Permission._EMP_BASIC)]
		[DataMember]
		[DisplayName("Entity HRBP")]
		//[ReadOnly(true)]
		public string EntityHRBP { get; set; }

		//[RequirePermission(Permission._EMP_BASIC)]
		[DataMember]
		[DisplayName("GBU HRBP")]
		//[ReadOnly(true)]
		public string GBUHRBP { get; set; }

		//[RequirePermission(Permission._EMP_BASIC)]
		[DataMember]
		[MultiChoice]
		[MasterReference("EntityMasters", typeof(EntityMaster), "EntityMasterID", "EntityName", new string[] { "IsActive", "Y" })]
		[DisplayName("Cross-Entity Manager")]
		public IEnumerable<int> CrossEntityManagers { get; set; }

		//[RequirePermission(Permission._EMP_BASIC)]
        private string entity;
		[DataMember]
		//[ReadOnly(true)]
		public string Entity
		{
			get
			{
				return DBHelper.GetCurrentEntity().EntityName;
			}
            set
            {
                entity = value; // stupid serializer 
            }
		}

		//[RequirePermission(Permission._EMP_BASIC)]
		[DataMember]
		[LocalProperty]
		[Hidden]
		public bool? RequiresApproval { get; set; }

		//[RequirePermission(Permission._EMP_BASIC)]
		[DataMember]
		[LocalProperty]
		[Hidden]
		public bool? RequiresEmployeeAction { get; set; }

        //[RequirePermission(Permission._EMP_BASIC)]
        [DataMember]
        [MasterProperty]
        [HideInCustomReports]
        [Hidden]
        public bool? EmployeeMustReviewSS { get; set; }

        //[RequirePermission(Permission._EMP_BASIC)]
        [DataMember]
        [MasterProperty]
        [HideInCustomReports]
        [Hidden]
        public bool? HRMustReviewSS { get; set; }

		[LocalProperty]
		//[RequirePermission(Permission._TERMINATION_INFO)]
		[DisplayName("Termination Period")]
		[Choices("1 Month", "2 Months", "3 Months", "6 Months", "12 Months")]
		[EntityRestriction(Entities._3iMind, Entities._4D, Entities.AGTBrazil, Entities.AGTChina, Entities.AGTGermany, Entities.AGTIndia, Entities.AGTNetherlands,
						   Entities.AGTSingapore, Entities.AGTSwitzerland, Entities.AGTUK, Entities.ATSD, Entities.Logic)]
		public string TerminationPeriod { get; set; }

		//[RequirePermission(Permission._EMP_ATTACH)]
		[DisplayName("Attachments")]
		public ModelCollection<AttachmentModel> Attachments { get; set; }

		//[RequirePermission(Permission._EMP_JOB)]
		[DisplayName("Job Assignments")]
		public JobAssignmentsCollection JobInfo { get; set; }

		//[RequirePermission(Permission._EMP_COMPENSATION)]
		[DisplayName("Compensation")]
		public ModelCollection<CompensationModel> CompensationInfo { get; set; }

		//[RequirePermission(Permission._EMP_COMP_BONUS)]
		[DisplayName("Bonus")]
		public ModelCollection<BonusModel> Bonuses { get; set; }

		//[RequirePermission(Permission._TRAINING)]
		public ModelCollection<ComplianceModel> Compliance { get; set; }

		//[RequirePermission(Permission._EMP_ALLOW)]
		public ModelCollection<AllowanceModel> Allowances { get; set; }

        public ModelCollection<AllowanceModel> AllowancePayments { get; set; }

		//[RequirePermission(Permission._EMP_CONTACT)]
		public ContactsCollection Contacts { get; set; }

		//[RequirePermission(Permission._EMP_ADDRESSES)]
		public AddressCollection Addresses { get; set; }

		//[RequirePermission(Permission._EDUCATION)]
		public ModelCollection<EducationModel> Education { get; set; }

		//[RequirePermission(Permission._EDUCATION)]
		public ModelCollection<TrainingModel> Training { get; set; }

		//[RequirePermission(Permission._LANGUAGES)]
		public ModelCollection<LanguageModel> Languages { get; set; }

		//[RequirePermission(Permission._DEPENDENTS)]
		public ModelCollection<DependentModel> Dependents { get; set; }

		//[RequirePermission(Permission._EMP_WRK_EXP)]
		[DisplayName("Work Experience")]
		public ModelCollection<WorkExperienceModel> WorkExperience { get; set; }

		//[RequirePermission(Permission._EMP_EM_CONTACT)]
		[DisplayName("Energency Contacts")]
		public ModelCollection<EmergencyContactModel> EmergencyContacts { get; set; }

		//[RequirePermission(Permission._BANK_INFO)]
		[DisplayName("Bank Info")]
		public ModelCollection<BankModel> BankInfo { get; set; }

		//[RequirePermission(Permission._EMP_REFERENCES)]
		public ModelCollection<ReferenceModel> References { get; set; }

		//[RequirePermission(Permission._EMP_TRAVEL_ID)]
		[DisplayName("Travel Documents")]
		public ModelCollection<TravelDocumentModel> TravelDocuments { get; set; }

		//[RequirePermission(Permission._TERMINATION_INFO)]
		public ModelCollection<TerminationModel> Terminations { get; set; }

        //[RequirePermission(Permission._TERMINATION_INFO)]
        public ModelCollection<TransferModel> Transfers { get; set; }

        //[RequirePermission(Permission._SECURITY_QUESTIONS)]
        [DisplayName("Security Questions")]
        public ModelCollection<SecurityQuestionModel> SecurityQuestions { get; set; }
		public JobAssignMasterModel CurrentJob
		{
			get
			{
				return this.JobInfo != null && this.JobInfo.Items != null ? this.JobInfo.Items.Where(x => x.IsActive == 'Y').FirstOrDefault() : null;
			}
		}
	}
}
