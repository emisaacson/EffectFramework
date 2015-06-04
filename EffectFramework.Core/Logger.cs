using Ninject;
using Ninject.Parameters;
using System;

namespace EffectFramework.Core
{
    /// <summary>
    /// Universal interface for logging.
    /// </summary>
    public class Logger
    {
        private ILoggingProvider _Log;

        public Logger(string ClassName)
        {
            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                _Log = Kernel.Get<ILoggingProvider>(new ConstructorArgument("ClassName", ClassName));
            }
        }

        public void Trace(string Template, params object[] objs)
        {
            _Log.Trace(Template, objs);
        }

        public void Debug(string Template, params object[] objs)
        {
            _Log.Debug(Template, objs);
        }

        public void Info(string Template, params object[] objs)
        {
            _Log.Info(Template, objs);
        }

        public void Warn(string Template, params object[] objs)
        {
            _Log.Warn(Template, objs);
        }

        public void Error(string Template, params object[] objs)
        {
            _Log.Error(Template, objs);
        }

        public void Fatal(string Template, params object[] objs)
        {
            _Log.Fatal(Template, objs);
        }

        public void Trace(string Template, Exception e)
        {
            _Log.Trace(Template, e);
        }

        public void Debug(string Template, Exception e)
        {
            _Log.Debug(Template, e);
        }

        public void Info(string Template, Exception e)
        {
            _Log.Info(Template, e);
        }

        public void Warn(string Template, Exception e)
        {
            _Log.Warn(Template, e);
        }

        public void Error(string Template, Exception e)
        {
            _Log.Error(Template, e);
        }

        public void Fatal(string Template, Exception e)
        {
            _Log.Fatal(Template, e);
        }
    }
}
