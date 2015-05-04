using HRMS.Core.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.ViewModels
{
    public class GetByAllowanceIdViewModel
    {
        public List<AllowancePaymentModel> AllowancePayments { get; set; }

        public decimal Total { get; set; }
    }
}