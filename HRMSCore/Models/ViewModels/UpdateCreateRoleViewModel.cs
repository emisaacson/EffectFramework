using Local.Models.Role;
using Local.Models.User;
using System.Collections.Generic;

namespace Local.Models.ViewModels
{
    public class UpdateCreateRoleViewModel
    {
        public RoleModel Role { get; set; }

        public List<AccessControllListModel> AccessControllList { get; set; }

        public List<UserModel> Users { get; set; }
    }
}