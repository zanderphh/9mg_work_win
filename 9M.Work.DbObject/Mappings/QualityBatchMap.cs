using _9M.Work.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.DbObject.Mappings
{
    public class QualityBatchMap : EntityTypeConfiguration<QualityBatchModel>
    {
        public QualityBatchMap()
        {
            this.ToTable("T_QualityCheck_Batch");
            this.HasKey(a => a.Id);
        }
    }
}
