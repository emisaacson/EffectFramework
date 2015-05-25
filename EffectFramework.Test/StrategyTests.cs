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
    public partial class EffectFrameworkTests
    {
        public class StrategyTests : IDisposable
        {

            public StrategyTests()
            {
                var configuration = new Configuration(BasePath)
                    .AddJsonFile("config.json");
                Configuration = configuration;

                Configure.ConnectionString = configuration["Data:DefaultConnection:ConnectionString"];

                PrepareEF7Database();
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
                TearDownEF7Database();
            }
        }
    }
}
