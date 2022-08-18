using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.DAL.Entities.Order_Aggregate;

namespace Talabat.DAL.Data.Configration
{
    internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(Item => Item.ItemOrdered, Product => Product.WithOwner());

            builder.Property(Item => Item.Price)
                .HasColumnType("decimal(18 ,2)");
        }
    }
}
