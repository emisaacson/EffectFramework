﻿using System;
using System.Collections.Generic;

namespace HRMS.Core.Models.Db
{
    public class Employee
    {
        public int? EmployeeID { get; set; }
        public int? EmployeeRecordID { get; set; }
        public bool IsDeleted { get; set; }
        public string DisplayName { get; set; }
        public Guid Guid { get; set; }

        public List<EmployeeRecord> EmployeeRecords { get; set; }
    }
}