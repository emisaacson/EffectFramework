using System;

namespace EffectFramework.Core.Models.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ApplyPolicyAttribute : Attribute
    {
        public UpdatePolicy Policy { get; private set; }
        public ApplyPolicyAttribute(Type Policy)
        {
            if (Policy == null)
            {
                throw new ArgumentNullException();
            }

            if (!typeof(UpdatePolicy).IsAssignableFrom(Policy))
            {
                throw new InvalidOperationException("Policy must be an assignable from UpdatePolicy.");
            }

            this.Policy = (UpdatePolicy)Activator.CreateInstance(Policy);
        }
    }
}
