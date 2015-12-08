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
        public static bool EnableLogging { get; set; } = false;

        public Logger(string ClassName)
        {
            if (!EnableLogging)
            {
                _Log = new NullLoggingProvider();
            }
            else
            {
                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    _Log = Kernel.Get<ILoggingProvider>(new ConstructorArgument("ClassName", ClassName));
                }
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

        public void Trace(Exception e, string Template, params object[] objs)
        {
            _Log.Trace(e, Template, objs);
        }

        public void Debug(Exception e, string Template, params object[] objs)
        {
            _Log.Debug(e, Template, objs);
        }

        public void Info(Exception e, string Template, params object[] objs)
        {
            _Log.Info(e, Template, objs);
        }

        public void Warn(Exception e, string Template, params object[] objs)
        {
            _Log.Warn(e, Template, objs);
        }

        public void Error(Exception e, string Template, params object[] objs)
        {
            _Log.Error(e, Template, objs);
        }

        public void Fatal(Exception e, string Template, params object[] objs)
        {
            _Log.Fatal(e, Template, objs);
        }
    }
}
