using System;

namespace HRMS.Core.Models.Db
{
    public class EntityField
    {
        public int EntityFieldID { get; set; }
        public int FieldTypeID { get; set; }
        public int EntityID { get; set; }
        public string ValueText { get; set; }
        public DateTime? ValueDate { get; set; }
        public Decimal? ValueDecimal { get; set; }
        public bool? ValueBoolean { get; set; }
        public int? ValueUser { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }

        public Entity Entity { get; set; }
    }
}
