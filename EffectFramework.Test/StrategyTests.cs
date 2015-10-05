using EffectFramework.Core;
using Microsoft.Framework.Configuration;
using Ninject;
using System;
using Xunit;

namespace EffectFramework.Test
{

    public class StrategyTests : IDisposable
    {
        public EffectFrameworkTestsContext ef { get; set; }
        public StrategyTests()
        {
            ef = new EffectFrameworkTestsContext();

            var Configuration = new ConfigurationBuilder()
                .SetBasePath(ef.BasePath)
                .AddJsonFile("config.json")
                .Build();
            ef.Configuration = Configuration;

            Configure.PersistenceConnectionString = ef.Configuration["Data:DefaultConnection:ConnectionString"];

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
            //ef.TearDownEF7Database();
        }
    }
}
