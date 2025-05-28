#if NET8_0_OR_GREATER
using System;
using System.Text.Json.Nodes;

namespace SimplySave {
    public class JsonSaver : Saver {

        private readonly bool loading;
        private readonly JsonObject value;

        private JsonSaver(SaverSettings settings, JsonObject value = null) : base(settings) {
            this.loading = value != null;
            this.value = value ?? new JsonObject();
        }

        /// <inheritdoc />
        protected override void DoAddKey(string key) {
            if (!this.loading)
                this.value.Add(this.Settings.KeyName ?? SaverSettings.DefaultKeyName, key);
        }

        /// <inheritdoc />
        protected override void DoAddObject<T>(T currentValue, Action<T> loader, Func<string, T> create, string name = null) {
            if (this.loading) {
                if (this.value.TryGetPropertyValue(name!, out var saved))
                    loader(JsonSaver.Load((JsonObject) saved, create));
            } else if (currentValue != null) {
                this.value.Add(name!, JsonSaver.Save(currentValue));
            }
        }

        /// <inheritdoc />
        protected override void DoAddValue<T>(T currentValue, Action<T> loader, string name = null) {
            if (this.loading) {
                if (this.value.TryGetPropertyValue(name!, out var saved))
                    loader(JsonSaver.GetValue<T>(saved));
            } else {
                this.value.Add(name!, JsonSaver.CreateValue(currentValue));
            }
        }

        /// <inheritdoc />
        protected override void DoAddObjects<T, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<string, T> createObj, Func<TCollection> createCollection, string name = null) {
            if (this.loading) {
                if (this.value.TryGetPropertyValue(name!, out var saved)) {
                    var collection = createCollection();
                    foreach (var entry in (JsonArray) saved)
                        collection.Add(entry != null ? JsonSaver.Load((JsonObject) entry, createObj) : default);
                    loader(collection);
                }
            } else if (currentValue != null) {
                var array = new JsonArray();
                foreach (var item in currentValue)
                    array.Add(item != null ? (JsonNode) JsonSaver.Save(item) : null);
                this.value.Add(name!, array);
            }
        }

        /// <inheritdoc />
        protected override void DoAddValues<T, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<TCollection> createCollection, string name = null) {
            if (this.loading) {
                if (this.value.TryGetPropertyValue(name!, out var saved)) {
                    var collection = createCollection();
                    foreach (var entry in (JsonArray) saved)
                        collection.Add(entry != null ? JsonSaver.GetValue<T>(entry) : default);
                    loader(collection);
                }
            } else if (currentValue != null) {
                var array = new JsonArray();
                foreach (var item in currentValue)
                    array.Add(item != null ? (JsonNode) JsonSaver.CreateValue(item) : null);
                this.value.Add(name!, array);
            }
        }

        /// <inheritdoc />
        protected override void DoAddObjects<TKey, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<string, TValue> createValue, Func<TDict> createDict, string name = null) {
            if (this.loading) {
                if (this.value.TryGetPropertyValue(name!, out var saved)) {
                    var dict = createDict();
                    foreach (var (key, val) in (JsonObject) saved)
                        dict.Add((TKey) Convert.ChangeType(key, typeof(TKey)), val != null ? JsonSaver.Load((JsonObject) val, createValue) : default);
                    loader(dict);
                }
            } else if (currentValue != null) {
                var obj = new JsonObject();
                foreach (var (key, val) in currentValue)
                    obj.Add(key.ToString()!, val != null ? JsonSaver.Save(val) : null);
                this.value.Add(name!, obj);
            }
        }

        /// <inheritdoc />
        protected override void DoAddValues<TKey, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<TDict> createDict, string name = null) {
            if (this.loading) {
                if (this.value.TryGetPropertyValue(name!, out var saved)) {
                    var dict = createDict();
                    foreach (var (key, val) in (JsonObject) saved)
                        dict.Add((TKey) Convert.ChangeType(key, typeof(TKey)), val != null ? JsonSaver.GetValue<TValue>(val) : default);
                    loader(dict);
                }
            } else if (currentValue != null) {
                var obj = new JsonObject();
                foreach (var (key, val) in currentValue)
                    obj.Add(key.ToString()!, val != null ? JsonSaver.CreateValue(val) : null);
                this.value.Add(name!, obj);
            }
        }

        public static T Load<T>(JsonObject data, Func<string, T> create, SaverSettings settings = default) where T : ISaveable {
            var key = data.TryGetPropertyValue(settings.KeyName ?? SaverSettings.DefaultKeyName, out var keyVal) ? keyVal.GetValue<string>() : null;
            var obj = create(key);
            obj.GetSaveData(new JsonSaver(settings, data));
            return obj;
        }

        public static JsonObject Save<T>(T obj, SaverSettings settings = default) where T : ISaveable {
            var saver = new JsonSaver(settings);
            obj.GetSaveData(saver);
            return saver.value;
        }

        private static JsonValue CreateValue<T>(T value) where T : IConvertible {
            return value switch {
                bool b => JsonValue.Create(b),
                byte b => JsonValue.Create(b),
                sbyte s => JsonValue.Create(s),
                char c => JsonValue.Create(c),
                decimal d => JsonValue.Create(d),
                double d => JsonValue.Create(d),
                float f => JsonValue.Create(f),
                int i => JsonValue.Create(i),
                uint u => JsonValue.Create(u),
                long l => JsonValue.Create(l),
                ulong u => JsonValue.Create(u),
                short s => JsonValue.Create(s),
                ushort u => JsonValue.Create(u),
                string s => JsonValue.Create(s),
                Enum e => JsonValue.Create(Convert.ToInt64(e)),
                null => null,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Cannot convert value {value} of type {typeof(T)} to JSON")
            };
        }

        private static T GetValue<T>(JsonNode node) where T : IConvertible {
            return typeof(T) switch {
                _ when typeof(T).IsEnum => (T) Enum.ToObject(typeof(T), node.GetValue<long>()),
                _ => node.GetValue<T>()
            };
        }

    }
}

#endif
