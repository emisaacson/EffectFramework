using Local.Classes.Attributes;
using Local.Classes.Helpers;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Reflection;
using Humanizer;

namespace Local.Models.Employee
{
    public class TemporaryTerminationModelFixMe : Model
    {
        //[RequirePermission(Permission._TERMINATION_INFO)]
        public ModelCollection<TerminationModel> Items { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        [DisplayName("Termination Period")]
        [Choices("1 Month", "2 Months", "3 Months", "6 Months", "12 Months")]
        public string TerminationPeriod { get; set; }

        public override void Validate()
        {
            List<ValidationResult> Errors = new List<ValidationResult>();

            if (this.Items != null)
            {
                IEnumerable<Model> Items = this.Items.Items;
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
 

            HRValidationResult Result = new HRValidationResult()
            {
                IsValid = Errors.Count() == 0,
                RawErrors = Errors,
            };

            _Validation = Result;
        }
    }
}