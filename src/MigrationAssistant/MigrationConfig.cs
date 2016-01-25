namespace MigrationAssistant
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The migration config.
    /// </summary>
    public class MigrationConfig
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationConfig"/> class with default values.
        /// </summary>
        public MigrationConfig()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationConfig"/> class and loads the supplied parameters as values.
        /// </summary>
        /// <param name="args">
        /// Parameters to load.
        /// </param>
        public MigrationConfig(List<string> args)
        {
            this.LoadFromArgs(args);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the database
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the migration ID
        /// </summary>
        public string Migration { get; set; }

        /// <summary>
        /// Gets or sets the database password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the address of the server instance
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Windows Authentication should be used against the database
        /// </summary>
        public bool TrustedConnection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether migrations should be enabled if they're not already enabled for the database.
        /// </summary>
        public bool ShouldEnableMigrations { get; set; }

        /// <summary>
        /// Gets or sets the database username
        /// </summary>
        public string Username { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Parses the commandline parameters.
        /// </summary>
        /// <param name="args">
        /// Commandline parameters
        /// </param>
        public void LoadFromArgs(List<string> args)
        {
            this.Database = args[0];
            this.Username = ValueForFlag(args, "-u", "--username") ?? "sa";
            this.Password = ValueForFlag(args, "-p", "--password") ?? string.Empty;
            this.Migration = ValueForFlag(args, "-m", "--migration");
            this.TrustedConnection = args.Contains("-t") || args.Contains("--trustedconnection");
            this.Server = ValueForFlag(args, "-s", "--server");
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the value for the first instance of a flag or alterative flag(s)
        /// </summary>
        /// <param name="args">
        /// The args to ssearch through
        /// </param>
        /// <param name="flags">
        /// The flag and variants to search for.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> value for the flag.
        /// </returns>
        private static string ValueForFlag(IList<string> args, params string[] flags)
        {
            var flag = flags.FirstOrDefault(args.Contains);

            return null != flag ? args[args.IndexOf(flag) + 1] : null;
        }

        #endregion Private Methods
    }
}