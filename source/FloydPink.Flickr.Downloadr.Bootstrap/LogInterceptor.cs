namespace FloydPink.Flickr.Downloadr.Bootstrap {
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Castle.DynamicProxy;
    using log4net;
    using ServiceStack.Text;

    public class LogInterceptor : IInterceptor {
        private readonly ILog _log;

        public LogInterceptor(ILog log) {
            this._log = log;
        }

        public ILog Log { get { return this._log; } }

        public void Intercept(IInvocation invocation) {
            if (Log.IsDebugEnabled) {
                Log.Debug(CreateInvocationLogString("Called", invocation));
            }
            try {
                invocation.Proceed();

                if (Log.IsDebugEnabled) {
                    var returnType = invocation.Method.ReturnType;
                    if (returnType != typeof (void)) {
                        var returnValue = invocation.ReturnValue;
                        if (returnType == typeof (Task)) {
                            Log.Debug("Returning with a task.");
                        } else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof (Task<>)) {
                            Log.Debug("Returning with a generic task.");
                            var task = (Task) returnValue;
                            task.ContinueWith(antecedent => {
                                                  var taskDescriptor = CreateInvocationLogString("Task from",
                                                      invocation);
                                                  var result =
                                                      antecedent.GetType()
                                                                .GetProperty("Result")
                                                                .GetValue(antecedent, null);
                                                  Log.Debug(taskDescriptor + " returning with: " +
                                                            DumpObject(result));
                                              });
                        } else {
                            Log.Debug("Returning with: " + DumpObject(returnValue));
                        }
                    }
                }
            }
            catch (Exception ex) {
                if (Log.IsErrorEnabled) {
                    Log.Error(CreateInvocationLogString("ERROR", invocation), ex);
                }
                throw;
            }
        }

        private static string CreateInvocationLogString(string operation, IInvocation invocation) {
            var sb = new StringBuilder(100);
            sb.AppendFormat("{0}: {1}.{2}(", operation, invocation.TargetType.Name, invocation.Method.Name);
            foreach (var argument in invocation.Arguments) {
                var argumentDescription = argument == null ? "null" : DumpObject(argument);
                sb.Append(argumentDescription).Append(",");
            }
            if (invocation.Arguments.Any()) {
                sb.Length--;
            }
            sb.Append(")");
            return sb.ToString();
        }

        private static string DumpObject(object argument) {
            if (argument == null) {
                return "<null>";
            }
            var objtype = argument.GetType();
            if (objtype == typeof (string) || objtype.IsPrimitive || !objtype.IsClass) {
                return argument.ToString();
            }

            return ToJson(argument);
        }

        public static string ToJson(object value) {
            try {
                return value.Dump();
            }
            catch (Exception) {
                return value.ToString();
            }
        }
    }
}
