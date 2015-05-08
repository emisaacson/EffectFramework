using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Core.Security
{
    public sealed class Permission
    {
        private readonly string Name;

        public static readonly Permission EMP_BASIC            = new Permission("EmpBasic");
        public static readonly Permission EMP_ATTACH           = new Permission("EmpAttach");
        public static readonly Permission EMP_ALLOW            = new Permission("EmpAllow");
        public static readonly Permission EMP_COMP             = new Permission("EmpComp");
        public static readonly Permission EMP_CONTACT          = new Permission("EmpContact");
        public static readonly Permission EMP_JOB              = new Permission("EmpbJob");
        public static readonly Permission EMP_TRAVEL_ID        = new Permission("EmpTravelID");
        public static readonly Permission VIEW_EMPLOYEES       = new Permission("ViewEmployees");
        public static readonly Permission EMP_EM_CONTACT       = new Permission("EmpEmContact");
        public static readonly Permission SYSTEM_LOGS          = new Permission("System Logs");
        public static readonly Permission SYSTEM_SETTINGS      = new Permission("SystemSettings");
        public static readonly Permission REPORTS              = new Permission("Reports");
        public static readonly Permission SEARCH_EMPLOYEES     = new Permission("SearchEmployees");
        public static readonly Permission DEPENDENTS           = new Permission("Dependents");
        public static readonly Permission EDUCATION            = new Permission("Education");
        public static readonly Permission LANGUAGES            = new Permission("Languages");
        public static readonly Permission TERMINATION_INFO     = new Permission("TerminationInfo");
        public static readonly Permission BANK_INFO            = new Permission("BankInfo");
        public static readonly Permission EMP_CREATE_EMPLOYEE  = new Permission("EmpCreateEmployee");
        public static readonly Permission EMP_COMPENSATION     = new Permission("EmpCompensation");
        public static readonly Permission EMP_WRK_EXP          = new Permission("EmpWrkExp");
        public static readonly Permission EMP_REFERENCES       = new Permission("EmpReferences");
        public static readonly Permission EMP_SKILLS           = new Permission("EmpSkills");
        public static readonly Permission EMP_ADDRESSES        = new Permission("EmpAddresses");
        public static readonly Permission EMP_COMP_BONUS       = new Permission("EmpCompBonus");
        public static readonly Permission EMP_EXEC_COMP        = new Permission("EmpExecComp");
        public static readonly Permission TRAINING             = new Permission("Training");
        public static readonly Permission SEND_EMP_EMAIL       = new Permission("SendEmpEmail");
        public static readonly Permission REQUEST_LETTERS      = new Permission("RequestLetters");
        public static readonly Permission PAYSLIPS             = new Permission("Payslips");
        public static readonly Permission SECURITY_QUESTIONS   = new Permission("SecurityQuestions");
        public static readonly Permission SECURITY_BRIEFING    = new Permission("SecurityBriefing");
        public static readonly Permission SHOW_ONLY_RESTRICTED = new Permission("ShowOnlyRestricted");
        public static readonly Permission EMP_ALLOWANCE_PAYMENTS = new Permission("EmpAllowancePayments");
        public static readonly Permission SHOW_COMPENSATION = new Permission("ShowCompensation");


        public const string _EMP_BASIC = "EmpBasic";
        public const string _EMP_ATTACH = "EmpAttach";
        public const string _EMP_ALLOW = "EmpAllow";
        public const string _EMP_COMP = "EmpComp";
        public const string _EMP_CONTACT = "EmpContact";
        public const string _EMP_JOB = "EmpbJob";
        public const string _EMP_TRAVEL_ID = "EmpTravelID";
        public const string _VIEW_EMPLOYEES = "ViewEmployees";
        public const string _EMP_EM_CONTACT = "EmpEmContact";
        public const string _SYSTEM_LOGS = "System Logs";
        public const string _SYSTEM_SETTINGS = "SystemSettings";
        public const string _REPORTS = "Reports";
        public const string _SEARCH_EMPLOYEES = "SearchEmployees";
        public const string _DEPENDENTS = "Dependents";
        public const string _EDUCATION = "Education";
        public const string _LANGUAGES = "Languages";
        public const string _TERMINATION_INFO = "TerminationInfo";
        public const string _BANK_INFO = "BankInfo";
        public const string _EMP_CREATE_EMPLOYEE = "EmpCreateEmployee";
        public const string _EMP_COMPENSATION = "EmpCompensation";
        public const string _EMP_WRK_EXP = "EmpWrkExp";
        public const string _EMP_REFERENCES = "EmpReferences";
        public const string _EMP_SKILLS = "EmpSkills";
        public const string _EMP_ADDRESSES = "EmpAddresses";
        public const string _EMP_COMP_BONUS = "EmpCompBonus";
        public const string _EMP_EXEC_COMP = "EmpExecComp";
        public const string _TRAINING = "Training";
        public const string _SEND_EMP_EMAIL = "SendEmpEmail";
        public const string _REQUEST_LETTERS = "RequestLetters";
        public const string _PAYSLIPS = "Payslips" ;
        public const string _SECURITY_QUESTIONS = "SecurityQuestions";
        public const string _SECURITY_BRIEFING = "SecurityBriefing";
        public const string _SHOW_ONLY_RESTRICTED = "ShowOnlyRestricted";
        public const string _EMP_ALLOWANCE_PAYMENT = "EmpAllowancePayments";
        public const string _SHOW_COMPENSATION = "ShowCompensation";

        private Permission(string Name) {
            this.Name = Name;
        }

        public override String ToString()
        {
            return Name;
        }
    }
}