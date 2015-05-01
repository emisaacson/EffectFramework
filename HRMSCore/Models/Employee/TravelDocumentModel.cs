using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Local.Classes.Attributes;
using Local.Classes.Security;
using HRMS.Modules.DBModel.Local;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Local.Classes.Helpers;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Local.Models.Employee
{
    [DisplayName("Identity Documents")]
    [LocalTable("EmpIDTravelDocs", typeof(EmpIDTravelDoc))]
    [EmpGeneralIDProperty("EmpID")]
    public class TravelDocumentModel : Model
    {
        [LocalProperty("TravelDocID")]
        //[RequirePermission(Permission._EMP_TRAVEL_ID)]
        [Key]
        [DisplayName("Travel Document ID")]
        //[SelfService]
        public int? TravelDocumentID { get; set; }

        //[RequirePermission(Permission._EMP_TRAVEL_ID)]
        //[ReadOnly(true)]
        //[SelfService]
        public bool? HasFile { get; set; }

        [LocalProperty("DocAssignment")]
        //[RequirePermission(Permission._EMP_TRAVEL_ID)]
        [DisplayName("Assigned To")]
        [Choices("Employee", "Dependent")]
        [MaxLength(50)]
        [Required]
        //[SelfService]
        public string AssignedTo { get; set; }

        [LocalProperty("DocType")]
        //[RequirePermission(Permission._EMP_TRAVEL_ID)]
        [DisplayName("Document Type")]
        [LocalReference("IDTravelDocTypes", typeof(IDTravelDocType), "TravelDocTypeID", "TypeName", new string[] { "IsActive", "Y" })]
        [Required]
        //[SelfService]
        public int? DocumentType { get; set; }

        [LocalProperty("DocIDNo")]
        //[RequirePermission(Permission._EMP_TRAVEL_ID)]
        [DisplayName("Document ID")]
        [MaxLength(50)]
        //[SelfService]
        public string DocumentIdentifier { get; set; }

        [LocalProperty("NameOnDoc")]
        //[RequirePermission(Permission._EMP_TRAVEL_ID)]
        [MaxLength(50)]
        [DisplayName("Name on Document")]
        //[SelfService]
        public string NameOnDocument { get; set; }

        [LocalProperty("DocIssueDate")]
        //[RequirePermission(Permission._EMP_TRAVEL_ID)]
        [DisplayName("Issue Date")]
        //[SelfService]
        public DateTime? IssueDate { get; set; }

        [LocalProperty("DocExpDate")]
        //[RequirePermission(Permission._EMP_TRAVEL_ID)]
        [DisplayName("Expiration Date")]
        //[SelfService]
        public DateTime? ExpirationDate { get; set; }

        [LocalProperty("DocIssuer")]
        //[RequirePermission(Permission._EMP_TRAVEL_ID)]
        [DisplayName("Issuer")]
        [MaxLength(50)]
        //[SelfService]
        public string Issuer { get; set; }

        [LocalProperty("DocIssuerCountry")]
        //[RequirePermission(Permission._EMP_TRAVEL_ID)]
        [DisplayName("Issuer Country")]
        [Choices(typeof(Countries), "ALL_COUNTRIES")]
        [MaxLength(50)]
        //[SelfService]
        public string IssuerCountry { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_TRAVEL_ID)]
        [Textarea]
        //[SelfService]
        public string Notes { get; set; }

        [LocalProperty]
        [EntityRestriction(Entities.AGTSwitzerland)]
        //[RequirePermission(Permission._EMP_TRAVEL_ID)]
        [Choices("B Aufenthaltsbewilligung EG/EFTA",
                 "C Niederlassungsbewilligung EG/EFTA",
                 "L Kurzaufenthaltsbewilligung EG/EFTA",
                 "G Grenzgängerbewilligung EG/EFTA",
                 "B Aufenthaltsbewilligung Drittstaatsangehörige",
                 "C Niederlassungsbewilligung Drittstaatsangehörige",
                 "L Kurzaufenthaltsbewilligung Drittstaatsangehörige",
                 "Meldeverfahren")]
        [MaxLength(100)]
        //[SelfService]
        public string SwissPermitType { get; set; }

        //[RequirePermission(Permission._EMP_TRAVEL_ID)]
        //[SelfService]
        public string ClientID { get; set; } // This property maps travel documents to posted files

        //[RequirePermission(Permission._EMP_TRAVEL_ID)]
        //[SelfService]
        [JsonIgnore]
        public HttpPostedFile File { get; set; }
    }
}
