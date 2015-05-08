using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMSCore.Helpers
{
    public class HRValidationResult
    {
        public bool IsValid { get; set; }
        public IEnumerable<ValidationResult> RawErrors { get; set; }
        public HRModelState Errors
        {
            get
            {
                Dictionary<string, string> Errors = new Dictionary<string, string>();
                int cnt = 0;
                foreach (var Error in RawErrors)
                {
                    foreach (string Field in Error.MemberNames)
                    {
                        Errors[Field+"["+cnt.ToString()+"]"] = Error.ErrorMessage;
                    }
                    cnt++;
                }

                return new HRModelState
                {
                    ModelState = Errors,
                };
            }
        }
    }
}