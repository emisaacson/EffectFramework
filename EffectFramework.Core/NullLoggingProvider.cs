using System;

namespace EffectFramework.Core
{
    /// <summary>
    /// Takes your logs and throws them away.
    /// </summary>
    public class NullLoggingProvider : ILoggingProvider
    {
        public void Debug(Exception e, string Template, params object[] objs)
        {

        }

        public void Debug(string Template, params object[] objs)
        {
            
        }

        public void Error(Exception e, string Template, params object[] objs)
        {

        }

        public void Error(string Template, params object[] objs)
        {
            
        }

        public void Fatal(Exception e, string Template, params object[] objs)
        {

        }

        public void Fatal(string Template, params object[] objs)
        {
            
        }

        public void Info(Exception e, string Template, params object[] objs)
        {

        }

        public void Info(string Template, params object[] objs)
        {
            
        }

        public void Trace(Exception e, string Template, params object[] objs)
        {

        }

        public void Trace(string Template, params object[] objs)
        {
            
        }

        public void Warn(Exception e, string Template, params object[] objs)
        {

        }

        public void Warn(string Template, params object[] objs)
        {
            
        }
    }
}
