using Local.Models.Role;
using Local.Models.User;
using System.Collections.Generic;


namespace Local.Models.ViewModels
{
    public class UpdateCreateUserAndAccessControlListViewModel : Model
    {
        public UserModel User { get; set; }
        public List<RoleModel> AllAvailableRoles { get; set; }
        public List<RoleModel> UserRoles { get; set; }
    }
}