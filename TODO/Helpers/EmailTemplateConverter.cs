using HRMS.Modules.DBModel;
using HRMS.Core.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HRMS.Core;

namespace HRMS.Core.Helpers
{
    public class EmailTemplateConverter
    {
        private EmployeeModel record ;
        private string template; 
        public EmailTemplateConverter(EmployeeModel empModel, string template)
        {
            this.record = empModel;
            this.template = template;
        }

        public string Process()
        {
            string result = string.Empty;
            try
            {
                result = Regex.Replace(template, "%.*?%", new MatchEvaluator(ComputeReplacement), RegexOptions.Singleline);
            }
            catch (ArgumentException ex)
            {
                // Syntax error in the regular expression
            }
            return result;
        }

        private string ComputeReplacement(Match match)
        {
            string value = match.Value.TrimStart('%').TrimEnd('%');
            string[] splitedToken = value.Split('.');
            var modelProperties = record.GetType().GetProperties();
            var property = modelProperties.FirstOrDefault(x => x.Name.ToLower() == splitedToken[1].ToLower().Trim());
            object resultObject = new object();
            string resultString = string.Empty;
            if(property==null)
            {
                return "<b>Wrong model field!</b>";
            }
            
            if(splitedToken.Length==2)
            {
                resultObject= property.GetValue(record);
                resultString = resultObject!=null? resultObject.ToString() : "%No data%";
            }
            else if(splitedToken.Length==3)
            {
                var subPropertyData = property.GetValue(record);
                if (subPropertyData == null) return "%No data%";
                var subproperty = subPropertyData.GetType().GetProperties().FirstOrDefault(x => x.Name == splitedToken[2]);
                if (subproperty == null)
                {
                    return "<b>Wrong model field!</b>";
                }
                resultObject =  subproperty.GetValue(subPropertyData);

                resultString = resultObject != null ? resultObject.ToString() : "%No data%";
            }
            if (resultObject != null)
            {
                if (resultObject.GetType() == typeof(DateTime))
                {
                    resultString = ((DateTime)resultObject).ToString("MMMM dd yyyy");
                }
                else if (resultObject.GetType() == typeof(DateTime?))
                {
                    DateTime? nd = resultObject as DateTime?;
                    if (nd != null && nd.HasValue)
                    {
                        resultString = nd.Value.ToString("MMMM dd yyyy");
                    }
                }
            }
            return resultString;
        }

        public static List<string> GetEmployeeModelTokens ()
        {
            List<string> tokens = new List<string>();
            
            var employeeProperties = typeof(EmployeeModel).GetProperties();
            foreach(var empProperty in employeeProperties)
            {
                string value = "%Employee.";
                if ((empProperty.PropertyType.IsGenericType || empProperty.PropertyType.BaseType.IsGenericType 
                    || empProperty.PropertyType == typeof(JobAssignmentsCollection)) && !empProperty.PropertyType.Name.Contains("Nullable"))// empProperty.PropertyType.IsSubclassOf(typeof(ModelCollection<>)))
                {
                    continue;
                }
                else if (empProperty.PropertyType.IsSubclassOf(typeof(Model)))
                    {
                        foreach (var subProperty in empProperty.PropertyType.GetProperties())
                        {

                            value= "%Employee." + empProperty.Name +  "."+subProperty.Name+"%";
                            tokens.Add(value);
                        }
                    }
                    else
                    {
                        value += empProperty.Name+ "%"; 
                        tokens.Add(value);
                    }
                }
                
            return tokens;

        }
             
    }
}