using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Local.Models
{
    public class ModelCollection<T> : Model
        where T : Model
    {
        public virtual IEnumerable<T> Items { get; set; }
    }
}