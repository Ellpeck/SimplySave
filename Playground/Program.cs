using System;
using System.Collections.Generic;
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
        new Obj {Value = 1.0f, Test = "First"},
        new Obj {Value = 2.0f, Test = "Second"}
    ],
    Strings = ["String1", "String2", "String3"],
    WeirdDict = new Dictionary<int, double> {
        {1, 1.1},
        {2, 2.2},
        {3, 3.3}
    },
};
Console.WriteLine(testObj);

var saved = JsonSaver.Save(testObj);
Console.WriteLine(saved);

var loaded = JsonSaver.Load(saved, Obj.CreateObj);
Console.WriteLine(loaded);

Console.WriteLine(loaded.ToString() == testObj.ToString());

namespace Playground {
    public class Obj : ISaveable {

        public Obj Nested;
        public float Value;
        public string Test;
        public List<Obj> Others;
        public List<string> Strings;
        public Dictionary<string, Obj> Dict;
        public Dictionary<int, double> WeirdDict;

        public void GetSaveData(Saver saver) {
            saver.AddKey("TinyLife.Obj");
            saver.AddObject(this.Nested, v => this.Nested = v);
            saver.AddValue(this.Value, v => this.Value = v);
            saver.AddValue(this.Test, v => this.Test = v);
            saver.AddObjects(this.Others, v => this.Others = v);
            saver.AddValues(this.Strings, v => this.Strings = v);
            saver.AddObjects(this.Dict, v => this.Dict = v);
            saver.AddValues(this.WeirdDict, v => this.WeirdDict = v);
        }

        public static Obj CreateObj(string key) {
            return key == "TinyLife.Obj" ? new Obj() : throw new ArgumentException($"Unknown type key: {key}");
        }

        public override string ToString() {
            return $"{nameof(this.Nested)}: {this.Nested}, {nameof(this.Value)}: {this.Value}, {nameof(this.Test)}: {this.Test}, {nameof(this.Others)}: {(this.Others != null ? string.Join(", ", this.Others) : "")} {nameof(this.Strings)}: {(this.Strings != null ? string.Join(", ", this.Strings) : "")} {nameof(this.Dict)}: {(this.Dict != null ? string.Join(", ", this.Dict) : "")} {nameof(this.WeirdDict)}: {(this.WeirdDict != null ? string.Join(", ", this.WeirdDict) : "")}";
        }

    }
}
