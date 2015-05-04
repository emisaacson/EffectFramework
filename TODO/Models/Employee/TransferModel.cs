using HRMS.Modules.DBModel;
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
    [DisplayName("Transfer")]
    [MasterTable("EmployeeEntityTransfers", typeof(EmployeeEntityTransfer))]
    public class TransferModel : Model
    {
        [Required]
        [MasterProperty]
        [DisplayName("Transfer ID")]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        [Key]
        public int TransferId { get; set; }

        [Required]
        [MasterProperty]
        [DisplayName("Source Database")]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        public string SourceDatabaseName { get; set; }

        [Required]
        [MasterProperty]
        [DisplayName("Source Local Employee ID")]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        public int? SourceEmpGeneralId { get; set; }

        [Required]
        [MasterProperty]
        [DisplayName("Destination Database")]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        public string DestinationDatabaseName { get; set; }

        [Required]
        [MasterProperty]
        [DisplayName("Destination Local Employee ID")]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        public int? DestinationEmpGeneralId { get; set; }

        [Required]
        [MasterProperty]
        [DisplayName("Source Employee ID")]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        public int? EmpMasterID { get; set; }

        [Required]
        [MasterProperty]
        [DisplayName("Destination Employee ID")]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        public int? NewEmpMasterID { get; set; }

        [Required]
        [MasterProperty]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DisplayName("Transfer Date")]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        public DateTime? CreateDateUtc { get; set; }

        //[RequirePermission(Permission._TERMINATION_INFO)]
        public string SourceEntityName
        {
            get
            {
                using (var Master = DBHelper.GetMasterDB())
                {
                    var Database = (SourceDatabaseName ?? "").Replace("[", "").Replace("]", "");
                    var Entity = Master.EntityMasters.Where(en => en.DBName == Database).FirstOrDefault();

                    return Entity != null ? Entity.EntityName : null;
                }
            }
        }

        //[RequirePermission(Permission._TERMINATION_INFO)]
        public string DestinationEntityName
        {
            get
            {
                using (var Master = DBHelper.GetMasterDB())
                {
                    var Database = (DestinationDatabaseName ?? "").Replace("[", "").Replace("]", "");
                    var Entity = Master.EntityMasters.Where(en => en.DBName == Database).FirstOrDefault();

                    return Entity != null ? Entity.EntityName : null;
                }
            }
        }
    }
}