# ExportDataToParquet

This is a .NET 6 console application that reads data from an MS SQL table and exports records older than three months to a Parquet file in a local folder.
Parquet offers efficient data compression and encoding, improving performance and reducing storage requirements compared to CSV and JSON, especially for large datasets.

Format	  Approx. Compressed Size	Approx.   Total Processing Time	Time Ratio (CSV = 1.0)
CSV	      8TB	High (slowest)	              1.0
JSON	    6-7TB	Medium	                    ~0.7
Parquet	  1-2TB	Low (fastest)	              ~0.3

## Table of Contents
- [Prerequisites](#prerequisites)
- [Setup](#setup)
  - [Create Database Table](#create-database-table)
  - [Update Connection String](#update-connection-string)
- [Build and Run](#build-and-run)

## Prerequisites
- .NET 6 SDK
- MS SQL Server
- NuGet packages: 
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.EntityFrameworkCore.SqlServer`
  - `Microsoft.EntityFrameworkCore.Tools`
  - `Parquet.Net`

## Setup

### Create Database Table

Run the following SQL script to create the `YourEntities` table and insert some sample data:

```sql
CREATE TABLE YourEntities (
    Id INT PRIMARY KEY IDENTITY(1,1),
    YourDateColumn DATETIME,
    OtherColumn NVARCHAR(100)
);

INSERT INTO YourEntities (YourDateColumn, OtherColumn) VALUES 
('2024-01-01', 'Sample Data 1'),
('2024-02-15', 'Sample Data 2'),
('2024-03-10', 'Sample Data 3'),
('2023-10-05', 'Sample Data 4'),
('2023-09-25', 'Sample Data 5');

Update Connection String
Update the connection string in the AppDbContext class in the Program.cs file with your actual database connection details:
optionsBuilder.UseSqlServer("Server=your_server_name;Database=your_database_name;User Id=your_username;Password=your_password;");


Build and Run
Open a terminal or command prompt and navigate to the directory containing these files.
Build and Run the project using the following command:
dotnet build
dotnet run

After running the application, you should see a message "Data export complete."
A file named export.parquet should be created in the current directory containing the data from the YourEntities table that is older than three months from the current date.

check the output file screanshot (in vs-code using available free extensions) :
https://github.com/chistysohail/ExportToParquet/blob/master/output_export_parquet_file.JPG
