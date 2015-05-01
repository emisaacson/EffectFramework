﻿using HRMS.Modules.DBModel.Local;
using Local.Classes.Attributes;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Local.Models.Internal
{
    [DisplayName("Currencies")]
    [LocalTable("Currencies", typeof(Currency))]
    public class CurrencyModel : Model
    {
        [DisplayName("Currency ID")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [LocalProperty("CurID")]
        [Key]
        public int? CurrencyID { get; set; }

        [DisplayName("Currency Name")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [LocalProperty("CurName")]
        public string CurrencyName { get; set; }
    }
}
