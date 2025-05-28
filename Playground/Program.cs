using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Playground;
using SimplySave;

var testObj = new Obj {
    Nested = new Obj {
        Value = 42.0f,
        Test = "Hello World",
        Strings = ["Nested String1", "Nested String2"],
        Dict = new Dictionary<string, Obj> {
            {"Key1", new Obj {Value = 10.0f, Test = "Nested Dict Value 1"}},
            {"Key2", new Obj {Value = 20.0f, Test = "Nested Dict Value 2"}}
        }
    },
    Value = 3.14f,
    Test = "Test String",
    Others = [
        new Obj {Value = 1.0f, Test = "First", OtherObj = new NonSaveableObj {Name = "First Non-Saveable"}},
        new Obj {Value = 2.0f, Test = "Second", Guid = Guid.NewGuid()}
    ],
    Strings = ["String1", "String2", "String3"],
    WeirdDict = new Dictionary<int, double> {
        {1, 1.1},
        {2, 2.2},
        {3, 3.3}
    },
};
Console.WriteLine(testObj);

var stopwatch = Stopwatch.StartNew();
var saved = JsonSaver.Save(testObj);
stopwatch.Stop();

Console.WriteLine($"Saved in {stopwatch.Elapsed.TotalMilliseconds}ms");
Console.WriteLine(saved);

stopwatch.Restart();
var loaded = JsonSaver.Load(saved, Obj.CreateObj);
stopwatch.Stop();

Console.WriteLine($"Loaded in {stopwatch.Elapsed.TotalMilliseconds}ms");
Console.WriteLine(loaded);

Console.WriteLine($"String equality: {loaded.ToString() == testObj.ToString()}");

stopwatch.Restart();
var (jsonObj, jsonLoaded) = (JsonSerializer.Serialize(testObj), JsonSerializer.Serialize(loaded));
stopwatch.Start();

Console.WriteLine($"System.Text.Json serialization took {stopwatch.Elapsed.TotalMilliseconds}ms");
Console.WriteLine($"System.Text.Json serialization equality: {jsonObj == jsonLoaded}");

namespace Playground {
    public class Obj : ISaveable {

        public Obj Nested;
        public float Value;
        public string Test;
        public List<Obj> Others;
        public List<string> Strings;
        public Dictionary<string, Obj> Dict;
        public Dictionary<int, double> WeirdDict;
        public Guid Guid;
        public List<Guid> Guids;
        public NonSaveableObj OtherObj;

        public void GetSaveData(Saver saver) {
            saver.AddKey("TinyLife.Obj");
            saver.AddObject(this.Nested, v => this.Nested = v);
            saver.AddValue(this.Value, v => this.Value = v);
            saver.AddValue(this.Test, v => this.Test = v);
            saver.AddObjects(this.Others, v => this.Others = v);
            saver.AddValues(this.Strings, v => this.Strings = v);
            saver.AddObjects(this.Dict, v => this.Dict = v);
            saver.AddValues(this.WeirdDict, v => this.WeirdDict = v);
            saver.AddValue(this.Guid, v => this.Guid = v, SaveableConverters.Guid);
            saver.AddValues(this.Guids, v => this.Guids = v, SaveableConverters.Guid);
            saver.AddObject(this.OtherObj, v => this.OtherObj = v, new NonSaveableObj.Wrapper());
        }

        public static Obj CreateObj(string key) {
            return key == "TinyLife.Obj" ? new Obj() : throw new ArgumentException($"Unknown type key: {key}");
        }

        public override string ToString() {
            return $"{nameof(this.Nested)}: {this.Nested}, {nameof(this.Value)}: {this.Value}, {nameof(this.Test)}: {this.Test}, {nameof(this.Others)}: {(this.Others != null ? string.Join(", ", this.Others) : "")} {nameof(this.Strings)}: {(this.Strings != null ? string.Join(", ", this.Strings) : "")} {nameof(this.Dict)}: {(this.Dict != null ? string.Join(", ", this.Dict) : "")} {nameof(this.WeirdDict)}: {(this.WeirdDict != null ? string.Join(", ", this.WeirdDict) : "")} {nameof(this.Guid)}: {this.Guid}";
        }

    }

    public class NonSaveableObj {

        public string Name;

        public class Wrapper : SaveableObjectConverter<Wrapper, NonSaveableObj>, ISaveable {

            public string SavedName;

            public override NonSaveableObj ConvertFromSaveable(Wrapper saveable) {
                return new NonSaveableObj {Name = this.SavedName};
            }

            public override Wrapper ConvertToSaveable(NonSaveableObj obj) {
                return obj != null ? new Wrapper {SavedName = obj.Name} : null;
            }

            public void GetSaveData(Saver saver) {
                saver.AddValue(this.SavedName, v => this.SavedName = v);
            }

        }

    }
}
