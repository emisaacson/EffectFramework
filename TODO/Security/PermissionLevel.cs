using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Core.Security
{
    public sealed class PermissionLevel
    {
        private readonly char Code;

        public static readonly PermissionLevel DELETE   = new PermissionLevel('D');
        public static readonly PermissionLevel CREATE   = new PermissionLevel('C');
        public static readonly PermissionLevel NOACCESS = new PermissionLevel('N');
        public static readonly PermissionLevel READ     = new PermissionLevel('R');

        public const string _DELETE = "DELETE";
        public const string _CREATE = "CREATE";
        public const string _NOACCESS = "NOACCESS";
        public const string _READ = "READ";

        private PermissionLevel(char Code)
        {
            this.Code = Code;
        }

        public char GetCode()
        {
            return Code;
        }
    }
}