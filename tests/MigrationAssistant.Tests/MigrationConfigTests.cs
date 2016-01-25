namespace MigrationAssistant.Tests
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MigrationConfigTests
    {
        /// <summary>
        /// Gets or sets the class under test.
        /// </summary>
        protected MigrationConfig UnderTest { get; set; }

        /// <summary>
        /// Runs test setup before each test.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            this.UnderTest = new MigrationConfig();
        }

        /// <summary>
        /// Cleans up after each test is run.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            this.UnderTest = null;
        }

        /// <summary>
        /// Tests that the config recognises the short flag "-t" for indicating a trusted connection to a server.
        /// </summary>
        [TestMethod]
        public void ShouldHandleShortTrustedConnectionFlag()
        {
            var args = new List<string> { "dbname", "-t" };

            this.UnderTest.LoadFromArgs(args);

            Assert.AreEqual("dbname", this.UnderTest.Database);
            Assert.AreEqual(true, this.UnderTest.TrustedConnection);
        }

        /// <summary>
        /// Tests that the config recognises the short flag "-t" for indicating a trusted connection to a server.
        /// </summary>
        [TestMethod]
        public void ShouldHandleLongTrustedConnectionFlag()
        {
            var args = new List<string> { "dbname", "--trustedconnection" };

            this.UnderTest.LoadFromArgs(args);

            Assert.AreEqual("dbname", this.UnderTest.Database);
            Assert.AreEqual(true, this.UnderTest.TrustedConnection);
        }

        /// <summary>
        /// Tests the the config can load a username and password conbination using short flags. ("-u" and "-p")
        /// </summary>
        [TestMethod]
        public void ShouldAcceptUsernameAndPasswordFromShortFlags()
        {
            var args = new List<string> { "dbname", "-u", "dbuser", "-p", "dbpass" };

            this.UnderTest.LoadFromArgs(args);

            Assert.AreEqual("dbname", this.UnderTest.Database);
            Assert.AreEqual("dbuser", this.UnderTest.Username);
            Assert.AreEqual("dbpass", this.UnderTest.Password);
        }

        /// <summary>
        /// Tests the the config can load a username and password conbination using long flags. ("--username" and "--password")
        /// </summary>
        [TestMethod]
        public void ShouldAcceptUsernameAndPasswordFromLongFlags()
        {
            var args = new List<string> { "dbname", "--username", "dbuser", "--password", "dbpass" };

            this.UnderTest.LoadFromArgs(args);

            Assert.AreEqual("dbname", this.UnderTest.Database);
            Assert.AreEqual("dbuser", this.UnderTest.Username);
            Assert.AreEqual("dbpass", this.UnderTest.Password);
        }

        /// <summary>
        /// Tests the config users the default "sa" user if only a password is specified.
        /// </summary>
        [TestMethod]
        public void ShouldUseDefaultUserWithSpecifiedPassword()
        {
            var args = new List<string> { "dbname", "--password", "dbpass" };

            this.UnderTest.LoadFromArgs(args);

            Assert.AreEqual("dbname", this.UnderTest.Database);
            Assert.AreEqual("sa", this.UnderTest.Username);
            Assert.AreEqual("dbpass", this.UnderTest.Password);
        }

        /// <summary>
        /// Tests that a servername can be specified alongside a trusted connection flag
        /// </summary>
        [TestMethod]
        public void ShouldAcceptTrustedConnectionAndServerName()
        {
            var args = new List<string> { "dbname", "-t", "-s", @".\sqlexpress" };

            this.UnderTest.LoadFromArgs(args);

            Assert.AreEqual("dbname", this.UnderTest.Database);
            Assert.AreEqual(true, this.UnderTest.TrustedConnection);
            Assert.AreEqual(@".\sqlexpress", this.UnderTest.Server);
        }

        [TestMethod]
        public void ShouldAcceptMigrationId()
        {
            var id = "201601251230.sql";

            var args = new List<string> { "dbname", "-t", "-m", id };

            this.UnderTest.LoadFromArgs(args);

            Assert.AreEqual(id, this.UnderTest.Migration);
        }
    }
}