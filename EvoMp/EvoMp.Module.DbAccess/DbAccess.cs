﻿using System;
using System.Collections.Generic;
using System.Linq;
using EvoMp.Module.Logger;

namespace EvoMp.Module.DbAccess
{
    public class DbAccess : IDbAccess
    {
        public DbAccess(ILogger logger)
        {
            string dbConnectionString = Environment.GetEnvironmentVariable("EvoMp_dbConnectionString");

            if (dbConnectionString == null)
                Environment.SetEnvironmentVariable("NameOrConnectionString",//EvoMp_GtMp
                    "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EvoMp_Test1;Integrated Security=True;" +
                    "Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;" +
                    "MultiSubnetFailover=False;MultipleActiveResultSets = True;");
            else
                Environment.SetEnvironmentVariable("NameOrConnectionString",
                    Environment.GetEnvironmentVariable("EvoMp_dbConnectionString"));

            List<string> connectionParameters = (Environment.GetEnvironmentVariable("NameOrConnectionString") ?? "").
                                                 Split(';').ToList();

            // Write console output
            logger.Write($"Using Database {connectionParameters.Find(s => s.ToLower().StartsWith("initial catalog"))?.Trim()}", 
                LogCase.System);
        }

        public string ModuleName { get; } = "DbAccess";

        public string ModuleDesc { get; } = "Provides connection string for entity framework";

        public string ModuleAuth { get; } = "Koka, Lukas";
    }
}