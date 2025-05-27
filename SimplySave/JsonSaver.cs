#if NET8_0_OR_GREATER
using System;
using System.Text.Json.Nodes;

namespace SimplySave {
    public class JsonSaver : Saver {

        private readonly bool loading;
        private readonly JsonObject value;

        public JsonSaver(JsonObject value = null) {
            this.loading = value != null;
            this.value = value ?? new JsonObject();
        }

        public JsonObject GetValue() {
            return (JsonObject) this.value.DeepClone();
        }

        /// <inheritdoc />
        public override void AddKey(string key) {
            if (!this.loading)
                this.value.Add("$type", key);
        }

        /// <inheritdoc />
        public override void Add<T>(T currentValue, Action<T> loader, Func<string, T> create, string name = null) {
            if (this.loading) {
                if (this.value.TryGetPropertyValue(Saver.StripName(name), out var saved))
                    loader(JsonSaver.LoadObject((JsonObject) saved, create));
            } else if (currentValue != null) {
                this.value.Add(Saver.StripName(name), JsonSaver.SaveObject(currentValue));
            }
        }

        /// <inheritdoc />
        public override void Add<T>(T currentValue, Action<T> loader, string name = null) {
            if (this.loading) {
                if (this.value.TryGetPropertyValue(Saver.StripName(name), out var saved))
                    loader(saved.GetValue<T>());
            } else {
                this.value.Add(Saver.StripName(name), JsonSaver.CreateValue(currentValue));
            }
        }

        /// <inheritdoc />
        public override void Add<T, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<string, T> createObj, Func<TCollection> createCollection, string name = null) {
            if (this.loading) {
                if (this.value.TryGetPropertyValue(Saver.StripName(name), out var saved)) {
                    var collection = createCollection();
                    foreach (var entry in (JsonArray) saved)
                        collection.Add(entry != null ? JsonSaver.LoadObject((JsonObject) entry, createObj) : default);
                    loader(collection);
                }
            } else if (currentValue != null) {
                var array = new JsonArray();
                foreach (var item in currentValue)
                    array.Add(item != null ? (JsonNode) JsonSaver.SaveObject(item) : null);
                this.value.Add(Saver.StripName(name), array);
            }
        }

        /// <inheritdoc />
        public override void Add<T, TCollection>(TCollection currentValue, Action<TCollection> loader, Func<TCollection> createCollection, string name = null) {
            if (this.loading) {
                if (this.value.TryGetPropertyValue(Saver.StripName(name), out var saved)) {
                    var collection = createCollection();
                    foreach (var entry in (JsonArray) saved)
                        collection.Add(entry != null ? entry.GetValue<T>() : default);
                    loader(collection);
                }
            } else if (currentValue != null) {
                var array = new JsonArray();
                foreach (var item in currentValue)
                    array.Add(item != null ? (JsonNode) JsonSaver.CreateValue(item) : null);
                this.value.Add(Saver.StripName(name), array);
            }
        }

        /// <inheritdoc />
        public override void Add<TKey, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<string, TValue> createValue, Func<TDict> createDict, string name = null) {
            if (this.loading) {
                if (this.value.TryGetPropertyValue(Saver.StripName(name), out var saved)) {
                    var dict = createDict();
                    foreach (var (key, val) in ((JsonObject) saved))
                        dict.Add((TKey) Convert.ChangeType(key, typeof(TKey)), val != null ? JsonSaver.LoadObject((JsonObject) val, createValue) : default);
                    loader(dict);
                }
            } else if (currentValue != null) {
                var obj = new JsonObject();
                foreach (var (key, val) in currentValue)
                    obj.Add(key.ToString()!, val != null ? JsonSaver.SaveObject(val) : null);
                this.value.Add(Saver.StripName(name), obj);
            }
        }

        /// <inheritdoc />
        public override void Add<TKey, TValue, TDict>(TDict currentValue, Action<TDict> loader, Func<TDict> createDict, string name = null) {
            if (this.loading) {
                if (this.value.TryGetPropertyValue(Saver.StripName(name), out var saved)) {
                    var dict = createDict();
                    foreach (var (key, val) in (JsonObject) saved)
                        dict.Add((TKey) Convert.ChangeType(key, typeof(TKey)), val != null ? val.GetValue<TValue>() : default);
                    loader(dict);
                }
            } else if (currentValue != null) {
                var obj = new JsonObject();
                foreach (var (key, val) in currentValue)
                    obj.Add(key.ToString()!, val != null ? JsonSaver.CreateValue(val) : null);
                this.value.Add(Saver.StripName(name), obj);
            }
        }

        private static T LoadObject<T>(JsonObject data, Func<string, T> create) where T : ISaveable {
            var key = data.TryGetPropertyValue("$type", out var keyVal) ? keyVal.GetValue<string>() : null;
            var obj = create(key);
            obj.GetSaveData(new JsonSaver(data));
            return obj;
        }

        private static JsonObject SaveObject<T>(T obj) where T : ISaveable {
            var saver = new JsonSaver();
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
                null => null,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Cannot convert value {value} of type {typeof(T)} to JSON")
            };
        }

    }
}

#endif
