using EffectFramework.Core;
using Microsoft.Framework.ConfigurationModel;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EffectFramework.Test
{

    public class StrategyTests : IDisposable
    {
        public EffectFrameworkTestsContext ef { get; set; }
        public StrategyTests()
        {
            ef = new EffectFrameworkTestsContext();

            var configuration = new Configuration(ef.BasePath)
                .AddJsonFile("config.json");
            ef.Configuration = configuration;

            Configure.ConnectionString = ef.Configuration["Data:DefaultConnection:ConnectionString"];

            ef.PrepareEF7Database();
        }

        [Fact]
        public void Test()
        {
            using (IKernel Kernel = new StandardKernel(new Configure()))
            {

            }
        }

        public void Dispose()
        {
            ef.TearDownEF7Database();
        }
    }
}
