using _9M.Work.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.DbObject.Mappings
{
    public class LastSyncExpressTimeMap : EntityTypeConfiguration<LastSyncExpressTimeModel>
    {
        public LastSyncExpressTimeMap()
        {
            this.ToTable("T_LastSyncExpressTime");
            this.HasKey(a => a.id); 
        }
    }
}
