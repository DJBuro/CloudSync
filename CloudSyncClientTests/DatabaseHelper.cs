﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace CloudSyncClientTests
{
    public class DatabaseHelper
    {
        public const string CreateACSDatabaseScriptFilename =
            @"C:\Andromeda Development Source\Rameses Web Components\DataAccess\AndroCloudDataAccessEntityFramework\CreateACSDatabase.sql";
        
        public static string CreateTestACSDatabase()
        {
            // Create a unique database name.  We can automatically delete test databases as they all start AT_ (Automated Test)
            string databaseName = "AT_ACS_" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "");
            string masterConnectionString = "Server=ROBDEVVM\\SQL2012;Database=master;Trusted_Connection=True;";
            string connectionString = "Server=ROBDEVVM\\SQL2012;Database=" + databaseName + ";Trusted_Connection=True;";
            string efConnectionString = "metadata=res://*/Model.ACS.csdl|res://*/Model.ACS.ssdl|res://*/Model.ACS.msl;provider=System.Data.SqlClient;provider connection string='data source=ROBDEVVM\\SQL2012;initial catalog=" + databaseName + ";persist security info=True;Trusted_Connection=True;multipleactiveresultsets=True;application name=EntityFramework'";

            // Get the SQL that we need to run
            string sql = "";
            using (StreamReader streamReader = new StreamReader(DatabaseHelper.CreateACSDatabaseScriptFilename))
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
                sql = sql.Replace("USE [ACS]\r\n;", "");

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
