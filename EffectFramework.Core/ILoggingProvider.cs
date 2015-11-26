using System;

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
        void Trace(Exception e, string Template, params object[] objs);
        void Debug(Exception e, string Template, params object[] objs);
        void Info(Exception e, string Template, params object[] objs);
        void Warn(Exception e, string Template, params object[] objs);
        void Error(Exception e, string Template, params object[] objs);
        void Fatal(Exception e, string Template, params object[] objs);
    }
}
