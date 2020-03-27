using EmailServices;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BakupDB
{
    public class GenerateScript
    {

        /// <summary>
        /// parameters of your database
        /// </summary>


        private readonly IConfiguration configuration;

        public GenerateScript(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ScriptDatabase()
        {
            var DatabaseConfiguration = this.configuration
            .GetSection("DatabaseConfiguration")
            .Get<DatabaseConfiguration>();

            string _serverName = DatabaseConfiguration.ServerName;
            string _database = DatabaseConfiguration.Database;
            string _username = DatabaseConfiguration.Username;
            string _password = DatabaseConfiguration.UserPassword;
            bool _logSecure = DatabaseConfiguration.TipeLogingSecure;
            SendEmail sendEmail = new SendEmail(this.configuration);

            try
            {
                ServerConnection srvConn2 = new ServerConnection(_serverName, _username, _password);   // connects to default instance  
                srvConn2.LoginSecure = _logSecure;
                Server srv3 = new Server(srvConn2);// connect remote
                var databse = srv3.Databases[_database]; // select database
                var scripter = new Scripter(srv3);
                //options generate database with OMS sql server example https://www.sqlservercentral.com/scripts/generate-all-table-script-by-using-smo-c
                scripter.Options.IncludeIfNotExists = false;
                scripter.Options.ScriptSchema = false;
                scripter.Options.ScriptData = true;
                scripter.Options.SchemaQualify = true;
                scripter.Options.ScriptForCreateDrop = true;
                //select compatibility in this case sql server 2017
                scripter.Options.TargetServerVersion = SqlServerVersion.Version140;
                scripter.Options.DriUniqueKeys = true;
                scripter.Options.DriForeignKeys = true;
                scripter.Options.Indexes = true;
                scripter.Options.DriPrimaryKey = true;
                scripter.Options.IncludeHeaders = true;
                scripter.Options.OptimizerData = true;

                string scrs = "";
                //create stream with data generate
                using var stream = new MemoryStream();
                using var writer = new StreamWriter(stream);
                foreach (Table myTable in databse.Tables)
                {
                    foreach (string s in scripter.EnumScript(new Urn[] { myTable.Urn
}))
                        scrs += s + "\n\n";
                }
                writer.WriteLine(scrs);
                writer.Flush();
                stream.Position = 0;
                //send stream in email
                sendEmail.Send("Backup data generate succeful.", stream);
            }
            catch (Exception ex)
            {
                sendEmail.Send(string.Format("Error to create backup data ex: ", ex));
            }
        }
    }
}
