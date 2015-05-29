using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core
{
    /// <summary>
    /// Interface for any adapter used to wire up a logger.
    /// </summary>
    public interface ILoggingProvider
    {
        void Trace(string Template, params object[] objs);
        void Debug(string Template, params object[] objs);
        void Info(string Template, params object[] objs);
        void Warn(string Template, params object[] objs);
        void Error(string Template, params object[] objs);
        void Fatal(string Template, params object[] objs);
        void Trace(string Template, Exception e);
        void Debug(string Template, Exception e);
        void Info(string Template,  Exception e);
        void Warn(string Template,  Exception e);
        void Error(string Template, Exception e);
        void Fatal(string Template, Exception e);
    }
}
