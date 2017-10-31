using _9M.Work.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.DbObject.Mappings
{
    public class ActivityMap : EntityTypeConfiguration<ActivityModel>
    {
        public ActivityMap()
        {
            this.ToTable("T_Activity");
            this.HasKey(a => a.Id);
            //活动与店铺的关系
            this.HasRequired(x => x.ShopModel).WithMany(x => x.ActivityList).HasForeignKey(x=>x.Shopid);
        }
    }
}
