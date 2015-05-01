using HRMS.Modules.DBModel.Local;
using Local.Classes.Attributes;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Local.Models.Internal
{
    [DisplayName("Travel Document Types")]
    [LocalTable("IDTravelDocTypes", typeof(IDTravelDocType))]
    public class TravelDocumentTypeModel : Model
    {
        [DisplayName("Education Type ID")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [LocalProperty("TravelDocTypeID")]
        [Key]
        public int? TravelDocumentTypeID { get; set; }

        [DisplayName("Travel Document Type")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [LocalProperty("TypeName")]
        public string TravelDocumentTypeName { get; set; }
    }
}