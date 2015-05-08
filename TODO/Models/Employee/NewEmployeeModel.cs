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
    public class NewEmployeeModel : IValidatableObject
    {
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("New Hire / Rehire")]
        [Choices("New Hire", "Re-Hire")]
        [DefaultValue("New Hire")]
        [Hidden]
        public string NewOrRehire { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Hidden]
        // This is populated in the case of rehire only
        public int? EmpMasterID { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Hidden]
        [DefaultValue("yes")]
        // This is populated in the case of guest only
        public string GuestAccount { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Middle Name")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public string MiddleName { get; set; }

        [DisplayName("Last Name")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public string LastName { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Active Directory User ID")]
        [Alphanumeric(ErrorMessage = "Must contain alphanumeric characters only.")]
        public string ADUserID { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Existing Email or Account Name")]
        public string ExistingADAccount { get; set; }

        [DisplayName("Create HRMS Account Only")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public bool HRMSAccountOnly { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Employee Type")]
        [Choices("0|Full Time Employee", "1|Part Time Employee", "3|Contractor", "4|Sub Contractor", "5|Consultant", "2|Intern (Work Student)", "6|Temporary Employee")]
        public int? Employment { get; set; }

        [DisplayName("Remote Developer / User Network Restriction to TFS Only")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public bool RemoteDeveloper { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Email Name")]
        [Alphanumeric(ErrorMessage = "Must contain alphanumeric characters only.")]
        public string EmailName { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Email Domain")]
        [Choices]
        public string EmailDomain { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Employee Level")]
        [Choices("Global Executive", "President", "Vice President", "Director", "Senior Manager", "Manager", "Technical-Team Lead", "Professional", "Individual")]
        public string EmployeeLevel { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Job Title")]
        public string JobTitle { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Direct Manager")]
        [Choices]
        public int? DirectManager { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public DateTime? EndDate { get; set; }

        [DisplayName("Personal Phone Number")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public string PersonalPhoneNumber { get; set; }

        [DisplayName("Personal Email")]
        [EmailAddress(ErrorMessage = "Must be an email address.")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public string PersonalEmail { get; set; }

        [DisplayName("Company")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices]
        public int? Company { get; set; }

        [DisplayName("Global Business Unit")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices]
        public int? GBU { get; set; }

        [DisplayName("Cost Center Code")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices]
        public int? CostCenter { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices]
        public int? Entity { get; set; }

        [DisplayName("Global Department Code")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices]
        public int? DeptCode { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices(typeof(Countries), "ALL_COUNTRIES")]
        public string Citizenship { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public DateTime? Birthdate { get; set; }

        [DisplayName("Worker Type")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices("Field Worker", "Office Worker")]
        public string WorkerType { get; set; }

        [DisplayName("Desktop Computer")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public bool? DesktopComputer { get; set; }

        [DisplayName("Office Location")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Choices(typeof(EmployeeFormHelper), "Locations")]
        public int? Location { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public bool? Laptop { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public bool? Keyboard { get; set; }

        [DisplayName("Laptop Bag")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public bool? LaptopBag { get; set; }

        [DisplayName("Desk Phone")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public bool? DeskPhone { get; set; }

        [DisplayName("Power Adapter")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public bool? PowerAdapter { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public bool? Monitor { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public bool? Dock { get; set; }

        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public bool? Mouse { get; set; }

        [DisplayName("RSA Token")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public bool? RSAToken { get; set; }

        [DisplayName("Mobile Phone")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        public bool? MobilePhone { get; set; }

        [DisplayName("Other Hardware")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Textarea]
        public string OtherHardware { get; set; }

        [DisplayName("Other Software")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Textarea]
        public string OtherSoftware { get; set; }

        [DisplayName("Additional Information")]
        //[RequirePermission(Permission._EMP_CREATE_EMPLOYEE)]
        [Textarea]
        public string AdditionalInformation { get; set; }

        public string ExistingADAccountReverseFullName
        {
            get
            {
                using (var Master = DBHelper.GetMasterDB())
                {
                    if (ExistingADAccount != null) {
                        var EmpMaster = Master.EmployeeMasters
                            .Where(x => ((x.ADDomain ?? "") + "\\" + (x.ADUserID ?? "")).ToLower() == ExistingADAccount.ToLower() && x.IsActive == 'Y')
                            .OrderBy(x => x.EmpStatus == "Active" ? 0 : 1)
                            .ThenByDescending(x => x.EmpMasterID)
                            .FirstOrDefault();

                        if (EmpMaster != null)
                        {
                            return EmpMaster.EmployeeReverseFullName;
                        }
                    }

                    return null;
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
                        var Location = Master.OfficeLocations.Where(x => x.OfficeLocationID == this.Location).FirstOrDefault();

                        if (Location != null)
                        {
                            return Location.ShortName;
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

        public string EmployeeTypeText
        {
            get
            {
                if (this.Employment.HasValue)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var Employment = Master.EmployeeTypes.Where(x => x.EmpTypeID == this.Employment).FirstOrDefault();

                        if (Employment != null)
                        {
                            return Employment.TypeName;
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

        public string EntityText
        {
            get
            {
                if (this.Entity.HasValue)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var Entity = Master.EntityMasters.Where(x => x.EntityMasterID == this.Entity).FirstOrDefault();

                        if (Entity != null)
                        {
                            return Entity.EntityName;
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

        public string ManagerText
        {
            get
            {
                if (this.DirectManager.HasValue)
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var Manager = Master.EmployeeMasters.Where(x => x.EmpMasterID == this.DirectManager).FirstOrDefault();

                        if (Manager != null)
                        {
                            var Lastname = !string.IsNullOrWhiteSpace(Manager.LNAlias) || !string.IsNullOrWhiteSpace(Manager.FNAlias) ?
                                           Manager.LNAlias ?? "" :
                                           Manager.LastName;

                            var Firstname = !string.IsNullOrWhiteSpace(Manager.LNAlias) || !string.IsNullOrWhiteSpace(Manager.FNAlias) ?
                                           Manager.FNAlias ?? "" :
                                           Manager.FirstName;

                            return Lastname + ", " + Firstname + " (" + (Manager.ADDomain ?? "") + "\\" + (Manager.ADUserID ?? "") + ")";
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(NewOrRehire))
            {
                yield return new ValidationResult("New Hire or Re-Hire field is required.", new string[] { "NewOrRehire" });
            }
            if (string.IsNullOrEmpty(FirstName))
            {
                yield return new ValidationResult("First Name field is required.", new string[] { "FirstName" });
            }
            if (string.IsNullOrEmpty(LastName))
            {
                yield return new ValidationResult("Last Name field is required.", new string[] { "LastName" });
            }
            if (!StartDate.HasValue)
            {
                yield return new ValidationResult("Start Date field is required.", new string[] { "StartDate" });
            }
            if (!Location.HasValue)
            {
                yield return new ValidationResult("Location field is required.", new string[] { "Location" });
            }
            if (!Location.HasValue)
            {
                yield return new ValidationResult("Location field is required.", new string[] { "Location" });
            }

            if (!HRMSAccountOnly)
            {
                if (string.IsNullOrEmpty(EmailName))
                {
                    yield return new ValidationResult("Email name field is required.", new string[] { "EmailName" });
                }
                if (string.IsNullOrEmpty(EmailDomain))
                {
                    yield return new ValidationResult("Email domain field is required.", new string[] { "EmailDomain" });
                }
            }

            // New Employee
            if (string.IsNullOrEmpty(ExistingADAccount) && string.IsNullOrEmpty(GuestAccount))
            {
                if (string.IsNullOrEmpty(ADUserID))
                {
                    yield return new ValidationResult("Active Directory User ID is required.", new string[] { "ADUserID" });
                }
                if (!Employment.HasValue)
                {
                    yield return new ValidationResult("Employment field is required.", new string[] { "Employment" });
                }
                if (!Company.HasValue)
                {
                    yield return new ValidationResult("Company field is required.", new string[] { "Company" });
                }
                if (!GBU.HasValue)
                {
                    yield return new ValidationResult("GBU field is required", new string[] { "GBU" });
                }
                if (!CostCenter.HasValue)
                {
                    yield return new ValidationResult("Cost Center field is required", new string[] { "CostCenter" });
                }
                if (!Entity.HasValue)
                {
                    yield return new ValidationResult("Entity field is required.", new string[] { "Entity" });
                }
                if (!DeptCode.HasValue)
                {
                    yield return new ValidationResult("Department field is required.", new string[] { "DeptCode" });
                }
                if (string.IsNullOrEmpty(WorkerType))
                {
                    yield return new ValidationResult("Worker type field is required.", new string[] { "WorkerType" });
                }
            }
            // New Guest Account
            else
            {
                if (string.IsNullOrEmpty(ExistingADAccount) && string.IsNullOrEmpty(GuestAccount))
                {
                    yield return new ValidationResult("Existing account field is required.", new string[] { "ExistingADAccount" });
                }
            }
        }
    }
}