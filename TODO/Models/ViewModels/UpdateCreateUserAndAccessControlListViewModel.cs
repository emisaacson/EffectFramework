using HRMS.Core.Models.Role;
using HRMS.Core.Models.Models;
using System.Collections.Generic;


namespace HRMS.Core.Models.ViewModels
{
    public class UpdateCreateUserAndAccessControlListViewModel : Model
    {
        public UserModel User { get; set; }
        public List<RoleModel> AllAvailableRoles { get; set; }
        public List<RoleModel> UserRoles { get; set; }
    }
}