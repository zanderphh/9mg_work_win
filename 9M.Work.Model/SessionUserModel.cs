using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class SessionUserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        //部门ID
        public int DeptId { get; set; }
        //是否超级管理员
        public bool IsAdmin { get; set; }
        //是否部门管理员
        public bool IsDeptAdmin { get; set; }
        //用户代称
        public string Alias { get; set; }
        public OnlineStoreAuthorization CShop { get; set; }
        public OnlineStoreAuthorization TmallMg { get; set; }
        public OnlineStoreAuthorization TmallMezingr { get; set; }
        public OnlineStoreAuthorization CAino { get; set; }
        public OnlineStoreAuthorization JDShop { get; set; }
        //权限
        public List<PermissionModel> PermissionList { get; set; }
    }

    public class OnlineStoreAuthorization 
    {
        public string Url { get; set; }
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
        public string SessionKey { get; set; }
    }

}
