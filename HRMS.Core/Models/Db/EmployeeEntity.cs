﻿using System;

namespace HRMS.Core.Models.Db
{
    public class EmployeeEntity
    {
        public int EmployeeEntityID { get; set; }
        public int EmployeeRecordID { get; set; }
        public int EntityID { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }

    }
}