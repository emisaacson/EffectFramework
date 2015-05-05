using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models.Fields;

namespace HRMS.Core.Models.Entities
{
    public class JobEntity : EntityBase
    {

        public override EntityType Type
        {
            get
            {
                return EntityType.Job;
            }
        }
        public readonly FieldString JobTitle = new FieldString(FieldType.Job_Title);
        public readonly FieldDate JobStartDate = new FieldDate(FieldType.Job_Start_Date);
    }
}
