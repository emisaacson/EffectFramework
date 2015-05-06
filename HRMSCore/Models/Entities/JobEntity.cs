using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models.Fields;
using HRMS.Core.Services;

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

        public JobEntity() : base() { }

        public JobEntity(IPersistenceService PersistenceService)
            : base(PersistenceService)
        {
            
        }

        protected override void WireUpFields()
        {
            JobTitle = new FieldString(FieldType.Job_Title, PersistenceService);
            JobStartDate = new FieldDate(FieldType.Job_Start_Date, PersistenceService);
        }

        public FieldString JobTitle { get; private set; }
        public FieldDate JobStartDate { get; private set; }
    }
}
