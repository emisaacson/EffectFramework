using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using HRMS.Core.Helpers;
using HRMS.Core.Security;
using HRMS.Core.Attributes;
using Humanizer;

namespace HRMS.Core.Models
{
    [Serializable]
    public class Model
    {

        public string GetDisplayName()
        {
            var CurrentType = this.GetType();

            if (Attribute.IsDefined(CurrentType, typeof(DisplayNameAttribute)))
            {
                return CurrentType.GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            }
            return CurrentType.Name;
        }


        public virtual void Validate()
        {

            List<ValidationResult> Errors = new List<ValidationResult>();

            if (this.GetType().GetProperty("Items") != null)
            {
                IEnumerable<Model> Items = (IEnumerable<Model>)this.GetType().GetProperty("Items").GetValue(this);
                int cnt = 1;
                foreach (Model Item in Items)
                {
                    var unlockedFields = Item.GetType().GetProperties().Where(x => DBHelper.GetCurrentUserContext().HasPermissionTo(x, PermissionLevel.CREATE));
                    foreach (var item in unlockedFields)
                    {
                        string DisplayName = item.Name;
                        if (Attribute.IsDefined(item, typeof(DisplayNameAttribute)))
                        {
                            var DNAttribute = item.GetCustomAttribute<DisplayNameAttribute>();
                            DisplayName = DNAttribute.DisplayName;
                        }
                        Validator.TryValidateProperty(item.GetValue(Item),
                            new ValidationContext(Item, null, null) { MemberName = item.Name, DisplayName = DisplayName + " (" + cnt.ToOrdinalWords() + " item)", },
                            Errors);
                    }
                    cnt++;
                }
            }
            else
            {
                var unlockedFields = this.GetType().GetProperties().Where(x => DBHelper.GetCurrentUserContext().HasPermissionTo(x, PermissionLevel.CREATE));
                foreach (var item in unlockedFields)
                {
                    Validator.TryValidateProperty(item.GetValue(this),
                        new ValidationContext(this, null, null) { MemberName = item.Name },
                        Errors);
                }
            }

            HRValidationResult Result = new HRValidationResult()
            {
                IsValid = Errors.Count() == 0,
                RawErrors = Errors,
            };

            _Validation = Result;
        }


        public bool IsValid()
        {
            if (_Validation == null)
            {
                Validate();
            }

            return _Validation.IsValid;
        }


        public HRModelState Errors()
        {
            if (_Validation == null)
            {
                Validate();
            }

            return _Validation.Errors;
        }

        [NonSerialized]
        protected HRValidationResult _Validation = null;
    }
}