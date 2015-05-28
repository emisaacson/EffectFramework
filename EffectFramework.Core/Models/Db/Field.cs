﻿using System;

namespace EffectFramework.Core.Models.Db
{
    public class Field
    {
        public int FieldID { get; set; }
        public int FieldTypeID { get; set; }
        public int EntityID { get; set; }
        public string ValueText { get; set; }
        public DateTime? ValueDate { get; set; }
        public Decimal? ValueDecimal { get; set; }
        public bool? ValueBoolean { get; set; }
        public int? ValueLookup { get; set; }
        public int? ValueItemReference { get; set; }
        public int? ValueEntityReference { get; set; }
        public byte[] ValueBinary { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }

        public Entity Entity { get; set; }
        public Lookup Lookup { get; set; }
    }
}
