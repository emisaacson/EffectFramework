using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models.Entities;
using HRMS.Core.Models.Fields;

namespace HRMS.Core.Models
{
    public class EmployeeRecord
    {
        public IEnumerable<EntityBase> AllEntities {
            get
            {
                List<EntityBase> Output = new List<EntityBase>();
                foreach (EntityType Entity in AllEntitiesByType.Keys)
                {
                    Output.AddRange(AllEntitiesByType[Entity]);
                }
                return Output;
            }
        }

        public int? EmployeeRecordID { get; private set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime EndEffectiveDate { get; set; }

        private Dictionary<EntityType, IEnumerable<EntityBase>> AllEntitiesByType;

        public EntityT GetFirstEntityOrDefault<EntityT>() where EntityT : EntityBase, new()
        {
            EntityT Instance = new EntityT();
            if (!AllEntitiesByType.ContainsKey(Instance.Type))
            {
                return default(EntityT);
            }

            return (EntityT)AllEntitiesByType[Instance.Type].First();
        }

        public IEnumerable<EntityT> GetAllEntitiesOrDefault<EntityT>() where EntityT : EntityBase, new()
        {
            EntityT Instance = new EntityT();
            if (!AllEntitiesByType.ContainsKey(Instance.Type))
            {
                return null;
            }

            return (IEnumerable<EntityT>)AllEntitiesByType[Instance.Type];
        }

        public EmployeeRecord()
        {

        }

        public EmployeeRecord(int EmployeeRecordID)
        {
            this.EmployeeRecordID = EmployeeRecordID;
            this.LoadByID(EmployeeRecordID);
        }

        private void LoadByID(int EmployeeRecordID)
        {

        }
    }
}
