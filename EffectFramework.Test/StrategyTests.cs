using EffectFramework.Core;
using Ninject;
using System;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace EffectFramework.Test
{

    public class StrategyTests : IDisposable
    {
        public EffectFrameworkTestsContext ef { get; set; }
        public StrategyTests()
        {
            ef = new EffectFrameworkTestsContext();

            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
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
