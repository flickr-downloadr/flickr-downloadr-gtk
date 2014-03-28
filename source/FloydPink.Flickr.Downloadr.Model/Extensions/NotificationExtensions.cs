using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace FloydPink.Flickr.Downloadr.Model.Extensions
{
    // http://stackoverflow.com/a/527840/218882
    public static class NotificationExtensions
    {
        #region Delegates

        /// <summary>
        ///     A property changed handler without the property name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sender">The object that raised the event.</param>
        public delegate void PropertyChangedHandler<TSender>(TSender sender);

        #endregion

        /// <summary>
        ///     Notifies listeners about a change.
        /// </summary>
        /// <param name="EventHandler">The event to raise.</param>
        /// <param name="Property">The property that changed.</param>
        public static void Notify(this PropertyChangedEventHandler EventHandler, Expression<Func<object>> Property)
        {
            // Check for null
            if (EventHandler == null)
                return;

            // Get property name
            var lambda = Property as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }
            var constantExpression = memberExpression.Expression as ConstantExpression;
            var propertyInfo = memberExpression.Member as PropertyInfo;

            // Invoke event
            foreach (Delegate del in EventHandler.GetInvocationList())
            {
                del.DynamicInvoke(new[]
                {
                    constantExpression.Value, new PropertyChangedEventArgs(propertyInfo.Name)
                });
            }
        }


        /// <summary>
        ///     Subscribe to changes in an object implementing INotifiyPropertyChanged.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ObjectThatNotifies">The object you are interested in.</param>
        /// <param name="Property">The property you are interested in.</param>
        /// <param name="Handler">The delegate that will handle the event.</param>
        public static void SubscribeToChange<T>(this T ObjectThatNotifies, Expression<Func<object>> Property,
            PropertyChangedHandler<T> Handler) where T : INotifyPropertyChanged
        {
            // Add a new PropertyChangedEventHandler
            ObjectThatNotifies.PropertyChanged += (s, e) =>
            {
                // Get name of Property
                var lambda = Property as LambdaExpression;
                MemberExpression memberExpression;
                if (lambda.Body is UnaryExpression)
                {
                    var unaryExpression = lambda.Body as UnaryExpression;
                    memberExpression =
                        unaryExpression.Operand as MemberExpression;
                }
                else
                {
                    memberExpression = lambda.Body as MemberExpression;
                }
                var propertyInfo = memberExpression.Member as PropertyInfo;

                // Notify handler if PropertyName is the one we were interested in
                if (e.PropertyName.Equals(propertyInfo.Name))
                {
                    Handler(ObjectThatNotifies);
                }
            };
        }
    }
}