using System;
using System.Collections.Generic;
using System.Globalization;
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace SimplySave {
    public abstract class Saver {

        public abstract void AddKey(string key);

        public abstract void Add<T>(T currentValue, Action<T> loader, Func<string, T> create,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : ISaveable;

        public abstract void Add<T>(T currentValue, Action<T> loader,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : IConvertible;

        public abstract void Add<T, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<string, T> createObj, Func<TCollection> createCollection,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : ISaveable where TCollection : ICollection<T>;

        public abstract void Add<T, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<TCollection> createCollection,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : IConvertible where TCollection : ICollection<T>;

        public abstract void Add<TKey, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<string, TValue> createValue, Func<TDict> createDict,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where TKey : IConvertible where TValue : ISaveable where TDict : IDictionary<TKey, TValue>;

        public abstract void Add<TKey, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<TDict> createDict,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where TKey : IConvertible where TValue : IConvertible where TDict : IDictionary<TKey, TValue>;

        public virtual void Add<T>(List<T> currentValue, Action<List<T>> loader, Func<string, T> createObj,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : ISaveable {
            this.Add(currentValue, loader, createObj, () => new List<T>(), name);
        }

        public virtual void Add<T>(List<T> currentValue, Action<List<T>> loader,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where T : IConvertible {
            this.Add<T, List<T>>(currentValue, loader, () => new List<T>(), name);
        }

        public virtual void Add<TKey, TValue>(Dictionary<TKey, TValue> currentValue, Action<Dictionary<TKey, TValue>> loader, Func<string, TValue> createValue,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where TKey : IConvertible where TValue : ISaveable {
            this.Add<TKey, TValue, Dictionary<TKey, TValue>>(currentValue, loader, createValue, () => new Dictionary<TKey, TValue>(), name);
        }

        public virtual void Add<TKey, TValue>(Dictionary<TKey, TValue> currentValue, Action<Dictionary<TKey, TValue>> loader,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) where TKey : IConvertible where TValue : IConvertible {
            this.Add<TKey, TValue, Dictionary<TKey, TValue>>(currentValue, loader, () => new Dictionary<TKey, TValue>(), name);
        }

        public virtual void Add(Guid currentValue, Action<Guid> loader,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) {
            this.Add(currentValue.ToString("D", CultureInfo.InvariantCulture), v => loader(Guid.Parse(v)), name);
        }

        public virtual void Add(TimeSpan currentValue, Action<TimeSpan> loader,
#if NET8_0_OR_GREATER
            [CallerArgumentExpression(nameof(currentValue))]
#endif
            string name = null) {
            this.Add(currentValue.ToString(null, CultureInfo.InvariantCulture), v => loader(TimeSpan.Parse(v, CultureInfo.InvariantCulture)), name);
        }

        protected static string StripName(string name) {
            if (name.StartsWith("this."))
                return name.Substring(5);
            return name;
        }

    }

    public interface ISaveable {

        void GetSaveData(Saver saver);

    }
}
