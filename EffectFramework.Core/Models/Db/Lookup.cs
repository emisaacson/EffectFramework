﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Db
{
    public class Lookup
    {
        public int LookupID { get; set; }
        public string Value { get; set; }
        public int LookupTypeID { get; set; }
        public bool IsDeleted { get; set; }

    }
}
