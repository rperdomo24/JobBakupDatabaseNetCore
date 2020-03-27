using BakupDB;
using EmailServices;
using JobBakupDatabaseNetCore.Services.Util;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobBakupDatabaseNetCore.Invocable
{
    public class JobBackup : IJobBackup
    {
        private readonly IConfiguration configuration;

        public JobBackup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Send()
        {
            try
            {
                GenerateScript generateScript = new GenerateScript(this.configuration);
                generateScript.ScriptDatabase();
            }
            catch (Exception ex)
            {
                //send excepcion to email
                SendEmail sendEmail = new SendEmail(configuration);
                sendEmail.Send(string.Format("Error to create, Excepcion description: {0}", ex.ToString()));
                throw;
            }
        }
    }
}
