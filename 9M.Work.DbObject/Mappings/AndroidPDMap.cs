using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _9M.Work.Model;
using System.Data.Entity.ModelConfiguration;

namespace _9M.Work.DbObject.Mappings
{

    public class AndroidPDMap : EntityTypeConfiguration<AndroidPDModel>
    {
        public AndroidPDMap()
        {
            this.ToTable("T_AndroidPD");
            this.HasKey(a => a.id);
        }
    }


}
