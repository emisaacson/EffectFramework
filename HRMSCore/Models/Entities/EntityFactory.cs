using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Core.Models.Entities
{
    public static class EntityFactory
    {
        public static EntityT GenerateEntityFromDbObject<EntityT>(Db.Entity Entity) where EntityT : EntityBase, new()
        {
            EntityT Instance = new EntityT();
            Instance.LoadUpEntity(Entity);
            return Instance;
        }
    }
}
