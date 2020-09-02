﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace nFBot.Core.Entities.Configurations
{
    public class FaqEntityConfiguration : IEntityTypeConfiguration<FaqEntity>
    {
        public void Configure(EntityTypeBuilder<FaqEntity> builder)
        {
            builder.HasKey(faq => faq.Id);
            builder.HasIndex(faq => faq.Tag).IsUnique();

            builder.Property(faq => faq.Tag).HasMaxLength(512).IsRequired();
            builder.Property(faq => faq.Content).IsRequired();
            builder.Property(faq => faq.Creator).IsRequired();
            builder.Property(faq => faq.CreatedDate).IsRequired();
        }
    }
}