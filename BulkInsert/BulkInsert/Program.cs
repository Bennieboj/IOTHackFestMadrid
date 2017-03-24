using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkInsert
{
    class Program
    {
        static void Main(string[] args)
        {
            string _testDbConnectionStringAzure = "Server=tcp:iotmadrid.database.windows.net,1433;Initial Catalog=IOTMadrid;Persist Security Info=False;User ID=iotmadrid;Password=IotH4ckF3est!;Connection Timeout=30;";
            var sqlConnection = new SqlConnection(_testDbConnectionStringAzure);
            SqlBulkCopy bulk = new SqlBulkCopy(sqlConnection);
            bulk.DestinationTableName = "AccessControlList";

            sqlConnection.Open();
            bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping("AccessDevice", "AccessDevice"));
            bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping("AccessDeviceType", "AccessDeviceType"));
            bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping("LocationId", "LocationId"));
            bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping("ServiceProfileId", "ServiceProfileId"));

            for (int j = 0; j < 920; j++)
            {
                var table = new DataTable();
                table.Columns.AddRange(new[] { new DataColumn("AccessDevice", typeof(string)), new DataColumn("AccessDeviceType", typeof(int)), new DataColumn("LocationId", typeof(int)), new DataColumn("ServiceProfileId", typeof(int)) });

                for (int i = 0; i < 100000; i++)
                {
                    var row = table.NewRow();
                    row.ItemArray = new object[] { Guid.NewGuid().ToString().ToUpperInvariant(), 1, 1, 1 };
                    table.Rows.Add(row);
                }
                bulk.WriteToServer(table);
            }
        }
    }
}
