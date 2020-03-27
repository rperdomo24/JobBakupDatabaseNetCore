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
            GenerateScript generateScript = new GenerateScript(this.configuration);
            generateScript.ScriptDatabase();

        }
    }
}
