namespace MigrationAssistant
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public class MigrationRunner : IDisposable
    {
        #region Protected Fields

        /// <summary>
        /// SQL Query to
        /// </summary>
        protected const string CheckIfMigrationsSetupSql =
            @"select count(*) as IsSetup from sys.tables where name = '__Migrations'";

        /// <summary>
        /// SQL Query to create the table to track migrations.
        /// </summary>
        protected const string CreateMigrationTableSql =
            @"create table __Migrations (Id nvarchar(100) not null primary key)";

        /// <summary>
        /// SQL Query to select existing migrations
        /// </summary>
        protected const string SelectExistingMigrationsSql = @"select Id, Name, from __Migrations";

        #endregion Protected Fields

        #region Public Constructors

        public MigrationRunner(MigrationConfig config)
        {
            this.Configuration = config;

            var auth = config.TrustedConnection ? "Trusted_Connection=true" : $"User Id={config.Username};Password={config.Password}";
            this.ConnectionString = $"Server={config.Server};Database={config.Database};{auth}";

            this.Connection = new SqlConnection(this.ConnectionString);
        }

        #endregion Public Constructors

        #region Public Properties

        public string ConnectionString { get; protected set; }

        #endregion Public Properties

        #region Protected Properties

        protected MigrationConfig Configuration { get; set; }
        protected SqlConnection Connection { get; set; }

        #endregion Protected Properties

        #region Public Methods

        public void Dispose()
        {
            this.Dispose(true);
        }

        public bool Run()
        {
            if (!this.CheckMigrationsEnabled())
            {
                if (this.Configuration.ShouldEnableMigrations)
                {
                    this.EnableMigrations();
                }
                else
                {
                    return false;
                }
            }

            var migrations = this.GetPendingMigrations();

            var originalColor = Console.ForegroundColor;
            foreach (var migration in migrations)
            {
                if (this.ExecuteMigration(migration))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[Success] ");
                    Console.ForegroundColor = originalColor;
                    Console.WriteLine($"{migration.Name} applied.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[Error] ");
                    Console.ForegroundColor = originalColor;
                    Console.WriteLine($"{migration.Name} was not applied. Terminating...");
                    return false;
                }
            }

            return true;
        }

        #endregion Public Methods

        #region Protected Methods

        protected bool CheckMigrationsEnabled()
        {
            using (var command = new SqlCommand(CheckIfMigrationsSetupSql, this.Connection))
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                return reader.GetInt32(0) > 0;
            }
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Connection?.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        protected void EnableMigrations()
        {
            using (var command = new SqlCommand(CreateMigrationTableSql, this.Connection))
            {
                command.ExecuteNonQuery();
            }
        }

        protected bool ExecuteMigration(FileInfo migrationFile)
        {
            var auth = this.Configuration.TrustedConnection ? "-E" : $"-U {this.Configuration.Username} -P {this.Configuration.Password}";
            var cmdArgs = $"{auth} -d {this.Configuration.Database} -i {migrationFile.FullName} -S {this.Configuration.Server}";

            var process = Process.Start("sqlcmd.exe", cmdArgs);
            return process?.WaitForExit(30000) ?? false;
        }

        protected IEnumerable<FileInfo> GetPendingMigrations()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            var files = directory.EnumerateFiles("*.sql");
            var applied = new List<string>();

            using (var command = new SqlCommand(SelectExistingMigrationsSql, this.Connection))
            using (var reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    applied.Add(reader.GetString(0));
                }
            }
            
            return files.Where(f => !applied.Contains(f.Name)).OrderBy(f => f.Name);
        }

        #endregion Protected Methods
    }
}