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
    public class CommentConfig : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("T_Comments");
            builder.Property(c=>c.Content).IsUnicode().IsRequired();
            builder.HasOne(c => c.C_Article).WithMany(c => c.Comments).HasForeignKey(c=>c.C_ArticleId).IsRequired();

        }
    }
}
