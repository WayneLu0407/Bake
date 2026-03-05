using System;
using System.Collections.Generic;
using Bake.Models;
using Bake.Models.Platform;
using Bake.Models.Sales;
using Bake.Models.Service;
using Bake.Models.Social;
using Bake.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Bake.Data;

public partial class BakeContext : DbContext
{

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("Bake"));
        }
    }
}
