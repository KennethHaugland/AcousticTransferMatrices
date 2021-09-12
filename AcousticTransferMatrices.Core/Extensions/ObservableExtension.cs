using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.Core.Extensions
{

    public static class ObservableExtensions
    {

            //public static IObservable<T> ObserveProperty<T, TValue>(this T source,
            //        Expression<Func<T, TValue>> propertyExpression
            //)
            //    where T : INotifyPropertyChanged
            //{
            //    return source.ObserveProperty(propertyExpression, false);
            //}

            //public static IObservable<T> ObserveProperty<T, TValue>(
            //    this T source,
            //    Expression<Func<T, TValue>> propertyExpression,
            //    bool observeInitialValue
            //)
            //    where T : INotifyPropertyChanged
            //{
            //    var memberExpression = (MemberExpression)propertyExpression.Body;

            //    var getter = propertyExpression.Compile();

            //    var observable = Observable
            //        .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
            //            h => new PropertyChangedEventHandler(h),
            //            h => source.PropertyChanged += h,
            //            h => source.PropertyChanged -= h)
            //        .Where(x => x.EventArgs.PropertyName == memberExpression.Member.Name)
            //        .Select(_ => getter(source));

            //    if (observeInitialValue)
            //        return observable.Merge(Observable.Return(getter(source)));

            //    return observable;
            //}

            //public static IObservable<T> ObservePropertyChanged(this T source)where T : INotifyPropertyChanged
            //{
            //    var observable = Observable
            //        .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
            //            h => new PropertyChangedEventHandler(h),
            //            h => source.PropertyChanged += h,
            //            h => source.PropertyChanged -= h)
            //        .Select(x => x.EventArgs.PropertyName);

            //    return observable;
            //}

            //public static IObservable ObserveCollectonChanged(this T source)
            //    where T : INotifyCollectionChanged
            //{
            //    var observable = Observable
            //        .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
            //            h => new NotifyCollectionChangedEventHandler(h),
            //            h => source.CollectionChanged += h,
            //            h => source.CollectionChanged -= h)
            //        .Select(_ => new Unit());

            //    return observable;
            //}

            //public static IObservable ObserveCollectonChanged(
            //     this T source, NotifyCollectionChangedAction collectionChangeAction)
            //            where T : INotifyCollectionChanged
            //{
            //    var observable = Observable
            //        .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
            //            h => new NotifyCollectionChangedEventHandler(h),
            //            h => source.CollectionChanged += h,
            //            h => source.CollectionChanged -= h)
            //        .Where(x => x.EventArgs.Action == collectionChangeAction)
            //        .Select(_ => new Unit());

            //    return observable;
            //}

        public static IObservable<Unit> AsUnit<TValue>(this IObservable<TValue> source)
        {
            return source.Select(x => new Unit());
        }

        public static IObservable<TItem> ObserveWeakly<TItem>(this IObservable<TItem> source)
        {
            return Observable.Create<TItem>(obs =>
            {
                var weakSubscription = new WeakSubscription<TItem>(source, obs);
                return () =>
                {
                    weakSubscription.Dispose();
                };
            });
        }


        public static IObservable<Unit> ObserveCollectonChanged<T>(this T source)
           where T : INotifyCollectionChanged
        {
            var observable = Observable
                .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    h => source.CollectionChanged += h,
                    h => source.CollectionChanged -= h)
                .AsUnit();

            return observable;
        }


        public static IObservable<Unit> ObserveCollectonChanged<T>(this T source, NotifyCollectionChangedAction collectionChangeAction)
           where T : INotifyCollectionChanged
        {
            var observable = Observable
                .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    h => source.CollectionChanged += h,
                    h => source.CollectionChanged -= h)
                .Where(x => x.Action == collectionChangeAction)
                .AsUnit();

            return observable;
        }

        public static IObservable<ItemChanged<T>> ItemChanged<T>(this ObservableCollection<T> collection, bool fireForExisting = false)
        {
            var observable = Observable.Create<ItemChanged<T>>(obs =>
            {
                void handler(object s, NotifyCollectionChangedEventArgs a)
                {
                    if (a.NewItems != null)
                    {
                        foreach (var item in a.NewItems.OfType<T>())
                        {
                            obs.OnNext(new ItemChanged<T>()
                            {
                                Item = item,
                                Added = true,
                                EventArgs = a
                            });
                        }
                    }
                    if (a.OldItems != null)
                    {
                        foreach (T item in a.OldItems.OfType<T>())
                        {
                            obs.OnNext(new ItemChanged<T>()
                            {
                                Item = item,
                                Added = false,
                                EventArgs = a
                            });
                        }
                    }
                }

                collection.CollectionChanged += handler;
                return () =>
                {
                    collection.CollectionChanged -= handler;
                };
            });

            if (fireForExisting)
                observable = observable.StartWith(Scheduler.CurrentThread, collection.Select(i => new ItemChanged<T>()
                {
                    Item = i,
                    Added = true,
                    EventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, i)
                }).ToArray());

            return observable;
        }


        public static IObservable<TObserved> ObserveInner<TItem, TObserved>(this ObservableCollection<TItem> collection, Func<TItem, IObservable<TObserved>> observe)
        {
            return Observable.Create<TObserved>(obs =>
            {
                Dictionary<TItem, IDisposable> subscriptions = new Dictionary<TItem, IDisposable>();

                var mainSubscription =
                    collection.ItemChanged(true)
                       .Subscribe(change =>
                       {
                           _ = subscriptions.TryGetValue(change.Item, out IDisposable subscription);
                           if (change.Added)
                           {
                               if (subscription == null)
                               {
                                   subscription = observe(change.Item).Subscribe(obs);
                                   subscriptions.Add(change.Item, subscription);
                               }
                           }
                           else
                           {
                               if (subscription != null)
                               {
                                   subscriptions.Remove(change.Item);
                                   subscription.Dispose();
                               }
                           }
                       });

                return () =>
                {
                    mainSubscription.Dispose();
                    foreach (KeyValuePair<TItem, IDisposable> subscription in subscriptions)
                    {
                        subscription.Value.Dispose();
                    }
                };
            });

        }

        public static IObservable<TValue> ObserveProperty<T, TValue>(this T source,
            Expression<Func<T, TValue>> propertyExpression) where T : INotifyPropertyChanged
        {
            return source.ObserveProperty(propertyExpression, false);
        }

        public static IObservable<TValue> ObserveProperty<T, TValue>(this T source,
            Expression<Func<T, TValue>> propertyExpression,
            bool observeInitialValue) where T : INotifyPropertyChanged
        {
            Func<T, TValue> getter = propertyExpression.Compile();

            IObservable<TValue> observable = Observable
                .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => source.PropertyChanged += h,
                    h => source.PropertyChanged -= h)
                .Where(x => x.PropertyName == propertyExpression.GetPropertyName())
                .Select(_ => getter(source));

            return observeInitialValue ? observable.Merge(Observable.Return(getter(source))) : observable;
        }


        public static IObservable<string> ObservePropertyChanged<T>(this T source)
           where T : INotifyPropertyChanged
        {
            IObservable<string> observable = Observable
                .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => source.PropertyChanged += h,
                    h => source.PropertyChanged -= h)
                .Select(x => x.PropertyName);

            return observable;
        }


        public static IObservable<ItemPropertyChangedEvent<TItem, TProperty>> ObservePropertyChanged<TItem, TProperty>(this TItem target, Expression<Func<TItem, TProperty>> propertyName, bool fireCurrentValue = false) where TItem : INotifyPropertyChanged
        {
            string property = ExpressionExtensions.GetPropertyName(propertyName);

            return ObservePropertyChanged(target, property, fireCurrentValue)
                   .Select(i => new ItemPropertyChangedEvent<TItem, TProperty>()
                   {
                       HasOld = i.HasOld,
                       NewValue = (TProperty)i.NewValue,
                       OldValue = i.OldValue == null ? default : (TProperty)i.OldValue,
                       Property = i.Property,
                       Sender = i.Sender
                   });
        }


        public static IObservable<ItemPropertyChangedEvent<TItem>> ObservePropertyChanged<TItem>(this TItem target, string propertyName = null, bool fireCurrentValue = false) where TItem : INotifyPropertyChanged
        {
            if (propertyName == null && fireCurrentValue)
            {
                throw new InvalidOperationException("You need to specify a propertyName if you want to fire the current value of your property");
            }

            return Observable.Create<ItemPropertyChangedEvent<TItem>>(obs =>
            {
                Dictionary<PropertyInfo, object> oldValues = new Dictionary<PropertyInfo, object>();
                Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
                void handler(object s, PropertyChangedEventArgs a)
                {
                    if (propertyName == null || propertyName == a.PropertyName)
                    {
                        if (!properties.TryGetValue(a.PropertyName, out PropertyInfo prop))
                        {
                            prop = typeof(TItem).GetProperty(a.PropertyName);
                            properties.Add(a.PropertyName, prop);
                        }
                        ItemPropertyChangedEvent<TItem> change = new ItemPropertyChangedEvent<TItem>()
                        {
                            Sender = target,
                            Property = prop,
                            NewValue = prop.GetValue(target, null)
                        };
                        if (oldValues.TryGetValue(prop, out object oldValue))
                        {
                            change.HasOld = true;
                            change.OldValue = oldValue;
                            oldValues[prop] = change.NewValue;
                        }
                        else
                        {
                            oldValues.Add(prop, change.NewValue);
                        }
                        obs.OnNext(change);
                    }
                }

                target.PropertyChanged += handler;

                if (propertyName != null && fireCurrentValue)
                    handler(target, new PropertyChangedEventArgs(propertyName));

                return () =>
                {
                    target.PropertyChanged -= handler;
                };
            });
        }
    }

    public static class ExpressionExtensions
    {

        public static string GetPropertyName<TProperty>(this Expression<Func<TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                if (expression.Body is UnaryExpression unaryExpression)
                {
                    if (unaryExpression.NodeType == ExpressionType.ArrayLength)
                        return "Length";
                    memberExpression = unaryExpression.Operand as MemberExpression;

                    if (memberExpression == null)
                    {
                        if (!(unaryExpression.Operand is MethodCallExpression methodCallExpression))
                            throw new NotImplementedException();

                        ConstantExpression arg = (ConstantExpression)methodCallExpression.Arguments[2];
                        return ((MethodInfo)arg.Value).Name;
                    }
                }
                else
                    throw new NotImplementedException();

            }

            string propertyName = memberExpression.Member.Name;
            return propertyName;

        }

        public static string GetPropertyName<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
            {
                if (expression.Body is UnaryExpression unaryExpression)
                {
                    if (unaryExpression.NodeType == ExpressionType.ArrayLength)
                        return "Length";
                    memberExpression = unaryExpression.Operand as MemberExpression;

                    if (memberExpression == null)
                    {
                        if (!(unaryExpression.Operand is MethodCallExpression methodCallExpression))
                            throw new NotImplementedException();

                        ConstantExpression arg = (ConstantExpression)methodCallExpression.Arguments[2];
                        return ((MethodInfo)arg.Value).Name;
                    }
                }
                else
                    throw new NotImplementedException();
            }
            var propertyName = memberExpression.Member.Name;
            return propertyName;

        }

        public static string PropertyToString<R>(this Expression<Func<R>> action)
        {
            MemberExpression ex = (MemberExpression)action.Body;
            return ex.Member.Name;
        }

        public static void CheckIsNotNull<R>(this Expression<Func<R>> action)
        {
            CheckIsNotNull(action, null);
        }

        public static void CheckIsNotNull<R>(this Expression<Func<R>> action, string message)
        {
            MemberExpression ex = (MemberExpression)action.Body;
            string memberName = ex.Member.Name;
            if (action.Compile()() == null)
            {
                throw new ArgumentNullException(memberName, message);
            }
        }

    }
}
