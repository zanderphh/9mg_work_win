using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class PermissionModel
    {
        public int Id { get; set; }
        public string Pname { get; set; }
        public string Url { get; set; }
        public int ParentId { get; set; }
        public string Ico { get; set; }
        public int OrderId { get; set; }
    }

    public class UserPermissionModel
    {
        public int Id { get; set; }
        public string Pname { get; set; }
        public string Url { get; set; }
        public int ParentId { get; set; }
        public string Ico { get; set; }
        public int OrderId { get; set; }
        public int CanDo { get; set; }
    }
}
