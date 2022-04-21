using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 多对多.Config
{
    public class T_Students_TeachersConfig : IEntityTypeConfiguration<T_Students_Teachers>
    {
        public void Configure(EntityTypeBuilder<T_Students_Teachers> builder)
        {
            builder.ToTable("T_Students_Teachers");


        }
    }
}
