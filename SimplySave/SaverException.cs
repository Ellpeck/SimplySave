using System;

namespace SimplySave {
    public class SaverException : Exception {

        public readonly Saver Saver;

        public SaverException(string message, Exception innerException, Saver saver) : base(message, innerException) {
            this.Saver = saver;
        }

    }

    public class SaverPropertyException : SaverException {

        public readonly string Name;
        public readonly object CurrentValue;
        public readonly object Loader;

        public SaverPropertyException(Exception innerException, Saver saver, string name, object currentValue, object loader) : base($"Failed to save or load property {name} with value {currentValue ?? "null"} and loader {loader ?? "null"}", innerException, saver) {
            this.Name = name;
            this.CurrentValue = currentValue;
            this.Loader = loader;
        }

    }
}
