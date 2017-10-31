using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class TemplateCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TemplateSizeModel
    {
        public int Id { get; set; }
        public string Size { get; set; }
        public string Label { get; set; }
        public int ClassId { get; set; }
    }

    public class TemplateSizeTitleModel
    {
        public int Id { get; set; }
        public string preportyName { get; set; }
        public string preportyCode { get; set; }
    }
}
