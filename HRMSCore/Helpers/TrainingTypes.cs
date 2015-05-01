using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Local.Classes.Helpers
{
    public static class TrainingTypes
    {
        public static readonly Dictionary<string, string> TrainingTypeMap = new Dictionary<string, string>
        {
            { "1", "Formal" },
            { "2", "Training" },
            { "3", "Certification" },
            { "4", "Associates Degree" },
            { "5", "Bachelors Degree" },
            { "6", "Master Degree" },
            { "7", "Professional Degree" },
            { "8", "PHD" },
            { "9", "Other" },
            { "10", "Apprenticeship" },
            { "11", "Professional Association" },
            { "12", "SC.D/M.D/MBBS" },
            { "13", "J.D (Law)" },
            { "14", "High School" }
        };
    }
}