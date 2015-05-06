using HRMS.Core.Services;

namespace HRMS.Core.Models.Entities
{
    public class AddressEntity : EntityBase
    {
        public override EntityType Type
        {
            get
            {
                return EntityType.Address;
            }
        }

        public AddressEntity() : base() { }

        public AddressEntity(IPersistenceService PersistenceService)
            : base(PersistenceService)
        {

        }

        protected override void WireUpFields()
        {
            
        }
    }
}
