﻿using HRMS.Modules.DBModel;
using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Unused
{
    [DisplayName("Global Business Unit")]
    [MasterTable("GBUCorpFuncs", typeof(GBUCorpFunc))]
    public class GBUModel : Model
    {
        [DisplayName("GBU ID")]
        //[RequirePermission(Permission._EMP_JOB)]
        [MasterProperty("GBUFuncID")]
        [Key]
        public int? GBUID { get; set; }

        [DisplayName("GBU Name")]
        //[RequirePermission(Permission._EMP_JOB)]
        [MasterProperty("GBUFuncName")]
        public string GBUName { get; set; }

		[DisplayName("GBU Type")]
		[MasterProperty("GbuType")]
		public string GBUType { get; set; }
    }
}