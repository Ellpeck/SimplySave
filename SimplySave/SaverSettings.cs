namespace SimplySave {
    public struct SaverSettings {

        public const string DefaultKeyName = "$type";

        public OnSaverException ExceptionHandler;
        public bool IgnoreUnhandledExceptions;
        public string KeyName;

    }

    public delegate void OnSaverException(Saver saver, SaverException exception, ref bool handled);
}
