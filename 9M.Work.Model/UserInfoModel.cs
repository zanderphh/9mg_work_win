using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class UserInfoModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public int DeptId { get; set; }
        public bool IsDeptAdmin { get; set; }
        public string Alias { get; set; }
    }
}
