using System;
using System.Collections.Generic;
using System.Globalization;
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace SimplySave {
    public abstract class Saver {

        public abstract void AddKey(string key);

        public abstract void AddObject<T>(T currentValue, Action<T> loader, Func<string, T> create,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : ISaveable;

        public abstract void AddValue<T>(T currentValue, Action<T> loader,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : IConvertible;

        public abstract void AddObjects<T, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<string, T> createObj, Func<TCollection> createCollection,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : ISaveable where TCollection : ICollection<T>;

        public abstract void AddValues<T, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<TCollection> createCollection,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : IConvertible where TCollection : ICollection<T>;

        public abstract void AddObjects<TKey, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<string, TValue> createValue, Func<TDict> createDict,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where TKey : IConvertible where TValue : ISaveable where TDict : IDictionary<TKey, TValue>;

        public abstract void AddValues<TKey, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<TDict> createDict,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where TKey : IConvertible where TValue : IConvertible where TDict : IDictionary<TKey, TValue>;

        // overloads with new() constraint

        public virtual void AddObject<T>(T currentValue, Action<T> loader,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : ISaveable, new() {
            this.AddObject(currentValue, loader, _ => new T(), name);
        }

        public virtual void AddObjects<T, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<TCollection> createCollection,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : ISaveable, new() where TCollection : ICollection<T> {
            this.AddObjects(currentValue, loader, _ => new T(), createCollection, name);
        }

        public virtual void AddObjects<TKey, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<TDict> createDict,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where TKey : IConvertible where TValue : ISaveable, new() where TDict : IDictionary<TKey, TValue> {
            this.AddObjects<TKey, TValue, TDict>(currentValue, loader, _ => new TValue(), createDict, name);
        }

        // convenience overloads

        public virtual void AddObjects<T>(List<T> currentValue, Action<List<T>> loader, Func<string, T> createObj,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : ISaveable {
            this.AddObjects(currentValue, loader, createObj, () => new List<T>(), name);
        }

        public virtual void AddValues<T>(List<T> currentValue, Action<List<T>> loader,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : IConvertible {
            this.AddValues<T, List<T>>(currentValue, loader, () => new List<T>(), name);
        }

        public virtual void AddObjects<TKey, TValue>(Dictionary<TKey, TValue> currentValue, Action<Dictionary<TKey, TValue>> loader, Func<string, TValue> createValue,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where TKey : IConvertible where TValue : ISaveable {
            this.AddObjects<TKey, TValue, Dictionary<TKey, TValue>>(currentValue, loader, createValue, () => new Dictionary<TKey, TValue>(), name);
        }

        public virtual void AddValues<TKey, TValue>(Dictionary<TKey, TValue> currentValue, Action<Dictionary<TKey, TValue>> loader,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where TKey : IConvertible where TValue : IConvertible {
            this.AddValues<TKey, TValue, Dictionary<TKey, TValue>>(currentValue, loader, () => new Dictionary<TKey, TValue>(), name);
        }

        public virtual void AddObjects<T>(List<T> currentValue, Action<List<T>> loader,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : ISaveable, new() {
            this.AddObjects<T, List<T>>(currentValue, loader, () => new List<T>(), name);
        }

        public virtual void AddObjects<TKey, TValue>(Dictionary<TKey, TValue> currentValue, Action<Dictionary<TKey, TValue>> loader,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where TKey : IConvertible where TValue : ISaveable, new() {
            this.AddObjects<TKey, TValue, Dictionary<TKey, TValue>>(currentValue, loader, () => new Dictionary<TKey, TValue>(), name);
        }

        // specific value overloads

        public virtual void AddValue(Guid currentValue, Action<Guid> loader,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) {
            this.AddValue(currentValue.ToString("D", CultureInfo.InvariantCulture), v => loader(Guid.Parse(v)), name);
        }

        public virtual void AddValue(TimeSpan currentValue, Action<TimeSpan> loader,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) {
            this.AddValue(currentValue.ToString(null, CultureInfo.InvariantCulture), v => loader(TimeSpan.Parse(v, CultureInfo.InvariantCulture)), name);
        }

        protected static string StripName(string name) {
            if (name.StartsWith("this."))
                return name.Substring(5);
            return name;
        }

    }
}
