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
        public IEnumerable<IEntity> AllEntities {
            get
            {
                List<IEntity> Output = new List<IEntity>();
                foreach (EntityType Entity in AllEntitiesByType.Keys)
                {
                    Output.AddRange(AllEntitiesByType[Entity]);
                }
                return Output;
            }
        }
        public DateTime EffectiveDate { get; set; }
        public DateTime EndEffectiveDate { get; set; }

        private Dictionary<EntityType, IEnumerable<IEntity>> AllEntitiesByType;

        public EntityT GetFirstEntityOrDefault<EntityT>() where EntityT : IEntity, new()
        {
            EntityT Instance = new EntityT();
            if (!AllEntitiesByType.ContainsKey(Instance.Type))
            {
                return default(EntityT);
            }

            return (EntityT)AllEntitiesByType[Instance.Type].First();
        }

        public IEnumerable<EntityT> GetAllEntitiesOrDefault<EntityT>() where EntityT : IEntity, new()
        {
            EntityT Instance = new EntityT();
            if (!AllEntitiesByType.ContainsKey(Instance.Type))
            {
                return null;
            }

            return (IEnumerable<EntityT>)AllEntitiesByType[Instance.Type];
        }
    }
}
