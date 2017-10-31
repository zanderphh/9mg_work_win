using _9M.Work.Model.Log;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.DbObject.Mappings
{
    public class GoodsLogMap : EntityTypeConfiguration<GoodsLogModel>
    {
        public GoodsLogMap()
        {
            this.ToTable("L_GoodsLog");
            this.HasKey(a => a.Id);
        }
    }
}
