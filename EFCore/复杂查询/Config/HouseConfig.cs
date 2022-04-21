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
    public class HouseConfig : IEntityTypeConfiguration<House>
    {
        public void Configure(EntityTypeBuilder<House> builder)
        {
            builder.ToTable("T_Houses");

            #region 乐观锁并发令牌配置
            //乐观锁配置：将被并发修改的属性（Owner）设置为并发令牌
            builder.Property(x => x.Owner).IsConcurrencyToken();

            #endregion

            #region RowVersion并发配置
            //RowVersion方式,处理并发
            builder.Property(x => x.RowVersion).IsRowVersion();

            #endregion


        }
    }
}
