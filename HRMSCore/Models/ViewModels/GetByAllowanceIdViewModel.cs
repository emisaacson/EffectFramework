using Local.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Local.Models.ViewModels
{
    public class GetByAllowanceIdViewModel
    {
        public List<AllowancePaymentModel> AllowancePayments { get; set; }

        public decimal Total { get; set; }
    }
}