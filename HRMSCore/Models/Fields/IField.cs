using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Core.Models.Fields
{
    public interface IField
    {
        object Value { get; set; }
        int? FieldID { get; }
        FieldType Type { get; }

        void LoadUpValues(FieldType Type, FieldBase Base);
    }
}
