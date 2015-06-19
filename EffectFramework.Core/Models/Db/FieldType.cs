using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Db
{
    public class FieldType
    {
        public int FieldTypeID { get; set; }
        public string Name { get; set; }
        public int DataTypeID { get; set; }
        public int LookupTypeID { get; set; }
        public int TenantID { get; set; }

    }
}
