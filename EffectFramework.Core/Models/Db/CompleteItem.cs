using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Db
{
    public class CompleteItem
    {
        public int ItemID { get; set; }
        public Guid ItemGuid { get; set; }
        public int ItemTenantID { get; set; }
        public int ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public int ItemTypeTenantID { get; set; }
        public int EntityID { get; set; }
        public int EntityTypeID { get; set; }
        public string EntityTypeName { get; set; }
        public int EntityTypeTenantID { get; set; }
        public DateTime EntityEffectiveDate { get; set; }
        public DateTime? EntityEndEffectiveDate { get; set; }
        public Guid EntityGuid { get; set; }
        public int EntityTenantID { get; set; }
        public int FieldTypeID { get; set; }
        public string FieldTypeName { get; set; }
        public int FieldTypeTenantID { get; set; }
        public int DataTypeID { get; set; }
        public string DataTypeName { get; set; }
        public int FieldID { get; set; }
        public string ValueText { get; set; }
        public DateTime? ValueDate { get; set; }
        public decimal? ValueDecimal { get; set; }
        public bool? ValueBoolean { get; set; }
        public int? ValueLookup { get; set; }
        public int? ValueItemReference { get; set; }
        public int? ValueEntityReference { get; set; }
        public byte[] ValueBinary { get; set; }
        public string LookupText { get; set; }
        public Guid EntityFieldGuid { get; set; }
        public int FieldTenantID { get; set; }
    }
}
