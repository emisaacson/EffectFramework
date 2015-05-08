using HRMS.Core.Models.Role;
using HRMS.Core.Models.Models;
using System.Collections.Generic;

namespace HRMS.Core.Models.ViewModels
{
    public class UpdateCreateRoleViewModel
    {
        public RoleModel Role { get; set; }

        public List<AccessControllListModel> AccessControllList { get; set; }

        public List<UserModel> Users { get; set; }
    }
}