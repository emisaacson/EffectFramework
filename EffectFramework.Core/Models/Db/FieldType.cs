namespace EffectFramework.Core.Models.Db
{
    public class FieldType
    {
        public long FieldTypeID { get; set; }
        public string Name { get; set; }
        public long DataTypeID { get; set; }
        public long? LookupTypeID { get; set; }
        public long TenantID { get; set; }

    }
}
