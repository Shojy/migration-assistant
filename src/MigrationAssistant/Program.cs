
namespace MigrationAssistant
{
    using System;
    using System.Linq;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point for application.
        /// </summary>
        /// <param name="args">
        /// Command line params
        /// </param>
        /// <remarks>
        /// Usage: migrate databasename -u sa -p password
        /// Usage: migrate databasename -m 201601251230
        ///  &lt;database&gt; Database name
        /// -m &lt;id&gt; Migration id, defaults to all migrations
        /// -u &lt;username&gt; Database username, defaults to sa if a password is provided, 
        ///     otherwise uses the current windows account.
        /// -p &lt;password&gt; Database password, defaults to blank
        /// </remarks>
        public static void Main(string[] args)
        {
            var config = new MigrationConfig(args.ToList());

            using (var runner = new MigrationRunner(config))
            {
                var success = runner.Run();

                Console.WriteLine(
                    success
                        ? "All pending migrations were applied successfully."
                        : "An error occured processing one of the migrations. Previous migrations were still applied.");
            }
        }
    }
}
