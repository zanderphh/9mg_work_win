﻿using _9M.Work.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.DbObject.Mappings
{
    public class PhotographyDetailMap : EntityTypeConfiguration<PhotographyDetailModel>
    {
        public PhotographyDetailMap()
        {
            this.ToTable("T_PhotographyDetail");
            this.HasKey(a => a.id);
        }
    }
}
