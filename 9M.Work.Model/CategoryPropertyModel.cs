using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class CategoryPropertyModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int PropertyType { get; set; }
        public string PropertyValue { get; set; }
    }
}
