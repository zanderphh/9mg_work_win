using _9M.Work.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.DbObject.Mappings
{
    public class ActivityLogMap : EntityTypeConfiguration<ActivityLogModel>
    {
        public ActivityLogMap()
        {
            this.ToTable("T_Activity_Log");
            this.HasKey(a => a.Id);
        }
    }
}
