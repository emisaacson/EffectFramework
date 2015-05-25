﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DefaultStrategyForDuplicateDatesAttribute : Attribute
    {
        public IUpdateStrategy Strategy { get; private set; }
        public DefaultStrategyForDuplicateDatesAttribute(Type Strategy)
        {
            if (Strategy == null)
            {
                throw new ArgumentNullException();
            }
            if (!typeof(IUpdateStrategy).IsAssignableFrom(Strategy))
            {
                throw new InvalidOperationException("Strategy must be assignable from IUpdateStrategy.");
            }

            this.Strategy = (IUpdateStrategy)Activator.CreateInstance(Strategy);
        }
    }
}
