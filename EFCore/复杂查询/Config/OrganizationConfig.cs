using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
    public class OrganizationConfig : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.ToTable("T_Organizations");
            builder.Property(c => c.Name).IsUnicode().IsRequired().HasMaxLength(100);
            builder.HasOne<Organization>(x => x.Parent).WithMany(x => x.Children);
            //全局查询筛选器
            builder.HasQueryFilter(x=>x.IsDeleted == false);

        }
    }
}
