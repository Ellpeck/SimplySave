using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SimplySave {
    public abstract class Saver {

        public readonly SaverSettings Settings;

        protected Saver(SaverSettings settings) {
            this.Settings = settings;
        }

        protected abstract void DoAddKey(string key);

        protected abstract void DoAddObject<T>(T currentValue, Action<T> loader, Func<string, T> create, [CallerArgumentExpression(nameof(currentValue))] string name = null) where T : ISaveable;

        protected abstract void DoAddValue<T>(T currentValue, Action<T> loader, [CallerArgumentExpression(nameof(currentValue))] string name = null) where T : IConvertible;

        protected abstract void DoAddObjects<T, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<string, T> createObj, Func<TCollection> createCollection, [CallerArgumentExpression(nameof(currentValue))] string name = null) where T : ISaveable where TCollection : ICollection<T>;

        protected abstract void DoAddValues<T, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<TCollection> createCollection, [CallerArgumentExpression(nameof(currentValue))] string name = null) where T : IConvertible where TCollection : ICollection<T>;

        protected abstract void DoAddObjects<TKey, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<string, TValue> createValue, Func<TDict> createDict, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TKey : IConvertible where TValue : ISaveable where TDict : IDictionary<TKey, TValue>;

        protected abstract void DoAddValues<TKey, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<TDict> createDict, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TKey : IConvertible where TValue : IConvertible where TDict : IDictionary<TKey, TValue>;

        public void AddKey(string key) {
            try {
                this.DoAddKey(key);
            } catch (Exception e) {
                this.OnSaverException(new SaverException($"Failed to add key {key}", e, this));
            }
        }

        public void AddObject<T>(T currentValue, Action<T> loader, Func<string, T> create, [CallerArgumentExpression(nameof(currentValue))] string name = null) where T : ISaveable {
            try {
                this.DoAddObject(currentValue, loader, create, Saver.StripName(name));
            } catch (Exception e) {
                this.OnPropertyException(e, Saver.StripName(name), currentValue, loader);
            }
        }

        public void AddValue<T>(T currentValue, Action<T> loader, [CallerArgumentExpression(nameof(currentValue))] string name = null) where T : IConvertible {
            try {
                this.DoAddValue(currentValue, loader, Saver.StripName(name));
            } catch (Exception e) {
                this.OnPropertyException(e, Saver.StripName(name), currentValue, loader);
            }
        }

        public void AddObjects<T, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<string, T> createObj, Func<TCollection> createCollection, [CallerArgumentExpression(nameof(currentValue))] string name = null) where T : ISaveable where TCollection : ICollection<T> {
            try {
                this.DoAddObjects(currentValue, loader, createObj, createCollection, Saver.StripName(name));
            } catch (Exception e) {
                this.OnPropertyException(e, Saver.StripName(name), currentValue, loader);
            }
        }

        public void AddValues<T, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<TCollection> createCollection, [CallerArgumentExpression(nameof(currentValue))] string name = null) where T : IConvertible where TCollection : ICollection<T> {
            try {
                this.DoAddValues<T, TCollection>(currentValue, loader, createCollection, Saver.StripName(name));
            } catch (Exception e) {
                this.OnPropertyException(e, Saver.StripName(name), currentValue, loader);
            }
        }

        public void AddObjects<TKey, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<string, TValue> createValue, Func<TDict> createDict, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TKey : IConvertible where TValue : ISaveable where TDict : IDictionary<TKey, TValue> {
            try {
                this.DoAddObjects<TKey, TValue, TDict>(currentValue, loader, createValue, createDict, Saver.StripName(name));
            } catch (Exception e) {
                this.OnPropertyException(e, Saver.StripName(name), currentValue, loader);
            }
        }

        public void AddValues<TKey, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<TDict> createDict, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TKey : IConvertible where TValue : IConvertible where TDict : IDictionary<TKey, TValue> {
            try {
                this.DoAddValues<TKey, TValue, TDict>(currentValue, loader, createDict, Saver.StripName(name));
            } catch (Exception e) {
                this.OnPropertyException(e, Saver.StripName(name), currentValue, loader);
            }
        }

        // overloads with converter

        public void AddObject<TSaveable, TObject>(TObject currentValue, Action<TObject> loader, ISaveableObjectConverter<TSaveable, TObject> converter, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TSaveable : ISaveable, new() {
            this.AddObject(converter.ConvertToSaveable(currentValue), v => loader(converter.ConvertFromSaveable(v)), _ => new TSaveable(), name);
        }

        public void AddValue<TValue, TObject>(TObject currentValue, Action<TObject> loader, ISaveableValueConverter<TValue, TObject> converter, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TValue : IConvertible {
            this.AddValue(converter.ConvertToValue(currentValue), v => loader(converter.ConvertFromValue(v)), name);
        }

        public void AddObjects<TSaveable, TObject, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<IEnumerable<TObject>, TCollection> createCollection, ISaveableObjectConverter<TSaveable, TObject> converter, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TSaveable : ISaveable, new() where TCollection : ICollection<TObject> {
            this.AddObjects(currentValue?.Select(converter.ConvertToSaveable).ToList(), v => loader(createCollection(v.Select(converter.ConvertFromSaveable))), _ => new TSaveable(), () => new List<TSaveable>(), name);
        }

        public void AddValues<TValue, TObject, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<IEnumerable<TObject>, TCollection> createCollection, ISaveableValueConverter<TValue, TObject> converter, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TValue : IConvertible where TCollection : ICollection<TObject> {
            this.AddValues<TValue, List<TValue>>(currentValue?.Select(converter.ConvertToValue).ToList(), v => loader(createCollection(v.Select(converter.ConvertFromValue))), () => new List<TValue>(), name);
        }

        public void AddObjects<TKeyValue, TKey, TValueSaveable, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<IEnumerable<KeyValuePair<TKey, TValue>>, TDict> createDict, ISaveableValueConverter<TKeyValue, TKey> keyConverter, ISaveableObjectConverter<TValueSaveable, TValue> valueConverter, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TKeyValue : IConvertible where TValueSaveable : ISaveable, new() where TDict : IDictionary<TKey, TValue> {
            this.AddObjects<TKeyValue, TValueSaveable, Dictionary<TKeyValue, TValueSaveable>>(currentValue?.ToDictionary(kv => keyConverter.ConvertToValue(kv.Key), kv => valueConverter.ConvertToSaveable(kv.Value)), v => loader(createDict(v.Select(kv => new KeyValuePair<TKey, TValue>(keyConverter.ConvertFromValue(kv.Key), valueConverter.ConvertFromSaveable(kv.Value))))), _ => new TValueSaveable(), () => new Dictionary<TKeyValue, TValueSaveable>(), name);
        }

        public void AddValues<TKeyValue, TKey, TValueValue, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<IEnumerable<KeyValuePair<TKey, TValue>>, TDict> createDict, ISaveableValueConverter<TKeyValue, TKey> keyConverter, ISaveableValueConverter<TValueValue, TValue> valueConverter, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TKeyValue : IConvertible where TValueValue : IConvertible where TDict : IDictionary<TKey, TValue> {
            this.AddValues<TKeyValue, TValueValue, Dictionary<TKeyValue, TValueValue>>(currentValue?.ToDictionary(kv => keyConverter.ConvertToValue(kv.Key), kv => valueConverter.ConvertToValue(kv.Value)), v => loader(createDict(v.Select(kv => new KeyValuePair<TKey, TValue>(keyConverter.ConvertFromValue(kv.Key), valueConverter.ConvertFromValue(kv.Value))))), () => new Dictionary<TKeyValue, TValueValue>(), name);
        }

        // overloads with new() constraint

        public void AddObject<T>(T currentValue, Action<T> loader, [CallerArgumentExpression(nameof(currentValue))] string name = null) where T : ISaveable, new() {
            this.AddObject(currentValue, loader, _ => new T(), name);
        }

        public void AddObjects<T, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<TCollection> createCollection, [CallerArgumentExpression(nameof(currentValue))] string name = null) where T : ISaveable, new() where TCollection : ICollection<T> {
            this.AddObjects(currentValue, loader, _ => new T(), createCollection, name);
        }

        public void AddObjects<TKey, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<TDict> createDict, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TKey : IConvertible where TValue : ISaveable, new() where TDict : IDictionary<TKey, TValue> {
            this.AddObjects<TKey, TValue, TDict>(currentValue, loader, _ => new TValue(), createDict, name);
        }

        // convenience overloads

        public void AddObjects<T>(List<T> currentValue, Action<List<T>> loader, Func<string, T> createObj, [CallerArgumentExpression(nameof(currentValue))] string name = null) where T : ISaveable {
            this.AddObjects(currentValue, loader, createObj, () => new List<T>(), name);
        }

        public void AddValues<T>(List<T> currentValue, Action<List<T>> loader, [CallerArgumentExpression(nameof(currentValue))] string name = null) where T : IConvertible {
            this.AddValues<T, List<T>>(currentValue, loader, () => new List<T>(), name);
        }

        public void AddObjects<TKey, TValue>(Dictionary<TKey, TValue> currentValue, Action<Dictionary<TKey, TValue>> loader, Func<string, TValue> createValue, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TKey : IConvertible where TValue : ISaveable {
            this.AddObjects<TKey, TValue, Dictionary<TKey, TValue>>(currentValue, loader, createValue, () => new Dictionary<TKey, TValue>(), name);
        }

        public void AddValues<TKey, TValue>(Dictionary<TKey, TValue> currentValue, Action<Dictionary<TKey, TValue>> loader, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TKey : IConvertible where TValue : IConvertible {
            this.AddValues<TKey, TValue, Dictionary<TKey, TValue>>(currentValue, loader, () => new Dictionary<TKey, TValue>(), name);
        }

        public void AddObjects<T>(List<T> currentValue, Action<List<T>> loader, [CallerArgumentExpression(nameof(currentValue))] string name = null) where T : ISaveable, new() {
            this.AddObjects<T, List<T>>(currentValue, loader, () => new List<T>(), name);
        }

        public void AddObjects<TKey, TValue>(Dictionary<TKey, TValue> currentValue, Action<Dictionary<TKey, TValue>> loader, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TKey : IConvertible where TValue : ISaveable, new() {
            this.AddObjects<TKey, TValue, Dictionary<TKey, TValue>>(currentValue, loader, () => new Dictionary<TKey, TValue>(), name);
        }

        public void AddObjects<TSaveable, TObject>(List<TObject> currentValue, Action<List<TObject>> loader, ISaveableObjectConverter<TSaveable, TObject> converter, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TSaveable : ISaveable, new() {
            this.AddObjects(currentValue?.Select(converter.ConvertToSaveable).ToList(), v => loader(v.Select(converter.ConvertFromSaveable).ToList()), _ => new TSaveable(), () => new List<TSaveable>(), name);
        }

        public void AddValues<TValue, TObject>(List<TObject> currentValue, Action<List<TObject>> loader, ISaveableValueConverter<TValue, TObject> converter, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TValue : IConvertible {
            this.AddValues<TValue, List<TValue>>(currentValue?.Select(converter.ConvertToValue).ToList(), v => loader(v.Select(converter.ConvertFromValue).ToList()), () => new List<TValue>(), name);
        }

        public void AddObjects<TKeyValue, TKey, TValueSaveable, TValue>(Dictionary<TKey, TValue> currentValue, Action<Dictionary<TKey, TValue>> loader, ISaveableValueConverter<TKeyValue, TKey> keyConverter, ISaveableObjectConverter<TValueSaveable, TValue> valueConverter, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TKeyValue : IConvertible where TValueSaveable : ISaveable, new() {
            this.AddObjects<TKeyValue, TValueSaveable, Dictionary<TKeyValue, TValueSaveable>>(currentValue?.ToDictionary(kv => keyConverter.ConvertToValue(kv.Key), kv => valueConverter.ConvertToSaveable(kv.Value)), v => loader(v.ToDictionary(kv => keyConverter.ConvertFromValue(kv.Key), kv => valueConverter.ConvertFromSaveable(kv.Value))), _ => new TValueSaveable(), () => new Dictionary<TKeyValue, TValueSaveable>(), name);
        }

        public void AddValues<TKeyValue, TKey, TValueValue, TValue>(Dictionary<TKey, TValue> currentValue, Action<Dictionary<TKey, TValue>> loader, ISaveableValueConverter<TKeyValue, TKey> keyConverter, ISaveableValueConverter<TValueValue, TValue> valueConverter, [CallerArgumentExpression(nameof(currentValue))] string name = null) where TKeyValue : IConvertible where TValueValue : IConvertible {
            this.AddValues<TKeyValue, TValueValue, Dictionary<TKeyValue, TValueValue>>(currentValue?.ToDictionary(kv => keyConverter.ConvertToValue(kv.Key), kv => valueConverter.ConvertToValue(kv.Value)), v => loader(v.ToDictionary(kv => keyConverter.ConvertFromValue(kv.Key), kv => valueConverter.ConvertFromValue(kv.Value))), () => new Dictionary<TKeyValue, TValueValue>(), name);
        }

        protected virtual void OnSaverException(SaverException exception) {
            var handled = false;
            this.Settings.ExceptionHandler?.Invoke(this, exception, ref handled);
            if (!handled && !this.Settings.IgnoreUnhandledExceptions)
                throw exception;
        }

        protected virtual void OnPropertyException(Exception exception, string name, object currentValue, object loader) {
            this.OnSaverException(new SaverPropertyException(exception, this, name, currentValue, loader));
        }

        private static string StripName(string name) {
            if (name.StartsWith("this."))
                return name.Substring(5);
            return name;
        }

    }

#if !NET8_0_OR_GREATER
    [AttributeUsage(AttributeTargets.Parameter)]
    internal class CallerArgumentExpressionAttribute : Attribute {

        public CallerArgumentExpressionAttribute(string parameterName) {}

    }
#endif
}
