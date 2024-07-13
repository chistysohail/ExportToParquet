using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Parquet;
using Parquet.Data;

namespace ImportDataFromParquet
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = ReadFromParquet();
            using (var context = new AppDbContext())
            {
                context.YourEntities.AddRange(data);
                context.SaveChanges();
            }

            Console.WriteLine("Data import complete.");
        }

        static List<YourEntity> ReadFromParquet()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "../../../../export.parquet");

            using (Stream fileStream = File.OpenRead(path))
            {
                using (var parquetReader = new ParquetReader(fileStream))
                {
                    var data = new List<YourEntity>();
                    for (int i = 0; i < parquetReader.RowGroupCount; i++)
                    {
                        using (ParquetRowGroupReader groupReader = parquetReader.OpenRowGroupReader(i))
                        {
                            DataColumn dateTimeColumn = groupReader.ReadColumn(new DataField<DateTimeOffset>("YourDateColumn"));
                            DataColumn otherColumn = groupReader.ReadColumn(new DataField<string>("OtherColumn"));

                            DateTimeOffset[] dateTimes = dateTimeColumn.Data.Cast<DateTimeOffset>().ToArray();
                            string[] others = otherColumn.Data.Cast<string>().ToArray();

                            for (int j = 0; j < dateTimes.Length; j++)
                            {
                                data.Add(new YourEntity
                                {
                                    YourDateColumn = dateTimes[j].DateTime,
                                    OtherColumn = others[j]
                                });
                            }
                        }
                    }
                    return data;
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
