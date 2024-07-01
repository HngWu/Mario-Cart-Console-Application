using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using MushroomPocket.NewFolder;
using MushroomPocket.Models;
using MushroomPocket.Model;

namespace MushroomPocket.Classes
{
    public class CharacterContext : DbContext
    {
        public DbSet<Character> Character { get; set; }

        public DbSet<Leaderboard> Leaderboard { get; set; }

        public CharacterContext(DbContextOptions<CharacterContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsbuilder)
        {
            string connection = @$"server=.\sqlexpress;database=character;trusted_connection=true;trustservercertificate=true;";
            if (!optionsbuilder.IsConfigured)
            {
                optionsbuilder
                    .UseSqlServer(
                   connection,
                    provideroptions => { provideroptions.EnableRetryOnFailure(); });
            }
        }
        public class CharacterContextFactory : IDesignTimeDbContextFactory<CharacterContext>
        {
            public CharacterContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<CharacterContext>();
                optionsBuilder.UseSqlServer(@$"server=.\sqlexpress;database=character;trusted_connection=true;trustservercertificate=true;"); // Or load from configuration

                return new CharacterContext(optionsBuilder.Options);
            }
        }

    }
}
