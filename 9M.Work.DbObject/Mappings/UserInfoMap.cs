using _9M.Work.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.DbObject.Mappings
{
    public class UserInfoMap : EntityTypeConfiguration<UserInfoModel>
    {
        public UserInfoMap()
        {
            this.ToTable("T_userinfo");
            this.HasKey(a => a.Id);
            //关系映射
            //this.HasRequired(a => a.Dept).WithMany(a => a.Users).HasForeignKey(a => a.DeptId);
            
        }
    }
}
