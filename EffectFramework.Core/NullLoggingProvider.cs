using System;

namespace EffectFramework.Core
{
    /// <summary>
    /// Takes your logs and throws them away.
    /// </summary>
    public class NullLoggingProvider : ILoggingProvider
    {
        public void Debug(string Template, Exception e)
        {

        }

        public void Debug(string Template, params object[] objs)
        {
            
        }

        public void Error(string Template, Exception e)
        {

        }

        public void Error(string Template, params object[] objs)
        {
            
        }

        public void Fatal(string Template, Exception e)
        {

        }

        public void Fatal(string Template, params object[] objs)
        {
            
        }

        public void Info(string Template, Exception e)
        {

        }

        public void Info(string Template, params object[] objs)
        {
            
        }

        public void Trace(string Template, Exception e)
        {

        }

        public void Trace(string Template, params object[] objs)
        {
            
        }

        public void Warn(string Template, Exception e)
        {

        }

        public void Warn(string Template, params object[] objs)
        {
            
        }
    }
}
