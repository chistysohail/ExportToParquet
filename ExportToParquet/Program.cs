using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Parquet;
using Parquet.Data;

namespace ExportDataToParquet
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new AppDbContext())
            {
                var threeMonthsAgo = DateTime.Now.AddMonths(-3);
                var dataToExport = context.YourEntities
                    .Where(e => e.YourDateColumn <= threeMonthsAgo)
                    .ToList();

                ExportToParquet(dataToExport);
            }

            Console.WriteLine("Data export complete.");
        }

        static void ExportToParquet(List<YourEntity> data)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "export.parquet");

            var dateTimeField = new DataField<DateTimeOffset>("YourDateColumn");
            var otherField = new DataField<string>("OtherColumn");

            var schema = new Schema(dateTimeField, otherField);
            var columns = new List<DataColumn>
            {
                new DataColumn(dateTimeField, data.Select(e => new DateTimeOffset(e.YourDateColumn)).ToArray()),
                new DataColumn(otherField, data.Select(e => e.OtherColumn).ToArray())
            };

            using (Stream fileStream = File.Create(path))
            {
                using (var parquetWriter = new ParquetWriter(schema, fileStream))
                {
                    using (ParquetRowGroupWriter groupWriter = parquetWriter.CreateRowGroup())
                    {
                        foreach (var column in columns)
                        {
                            groupWriter.WriteColumn(column);
                        }
                    }
                }
            }
        }
    }

    public class AppDbContext : DbContext
    {
        public DbSet<YourEntity> YourEntities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(LocalDB)\\MSSQLLocalDB;Database=Test;Trusted_Connection=True;MultipleActiveResultSets=true;");
        }
    }

    public class YourEntity
    {
        public int Id { get; set; }
        public DateTime YourDateColumn { get; set; }
        public string OtherColumn { get; set; }
    }
}
