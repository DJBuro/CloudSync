using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace CloudSyncTests
{
    public class DatabaseHelper
    {
        public const string CreateAndroAdminDatabaseScriptFilename = 
            @"C:\Andromeda Development Source\Rameses Web Components\DataAccess\AndroAdminDataAccess\EntityFramework\CreateAndroAdminDatabase.sql";
        
        public static string CreateTestAndroAdminDatabase()
        {
            // Create a unique database name.  We can automatically delete test databases as they all start AT_ (Automated Test)
            string databaseName = "AT_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "");
            string masterConnectionString = "Server=ROBDEVVM\\SQL2012;Database=master;Trusted_Connection=True;";
            string connectionString = "Server=ROBDEVVM\\SQL2012;Database=" + databaseName + ";Trusted_Connection=True;";
            string efConnectionString = "metadata=res://*/EntityFramework.EntityModel.csdl|res://*/EntityFramework.EntityModel.ssdl|res://*/EntityFramework.EntityModel.msl;provider=System.Data.SqlClient;provider connection string='data source=ROBDEVVM\\SQL2012;initial catalog=" + databaseName + ";persist security info=True;Trusted_Connection=True;multipleactiveresultsets=True;application name=EntityFramework'";

            // Get the SQL that we need to run
            string sql = "";
            using (StreamReader streamReader = new StreamReader(DatabaseHelper.CreateAndroAdminDatabaseScriptFilename))
            {
                sql = streamReader.ReadToEnd();
            }

            // Create a blank database
            using (SqlConnection sqlConnection = new SqlConnection(masterConnectionString))
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                {
                    // Create a blank database
                    sqlCommand.CommandText = "create database [" + databaseName + "]";

                    // Run the SQL
                    sqlCommand.ExecuteNonQuery();
                }
            }

            // Create the database tables
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                // SQL management studio generates databse schema scripts with GOs but they are not valid SQL statements
                // Replace all GOs with semi colons
                sql = sql.Replace("GO", ";");

                // Make sure the script doesn't have any usings
                sql = sql.Replace("USE [AndroAdmin]\r\n;", "");

                using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                {
                    // Create the database tables
                    sqlCommand.CommandText = sql;

                    // Run the SQL
                    sqlCommand.ExecuteNonQuery();
                }
            }

            return efConnectionString;
        }
    }
}
