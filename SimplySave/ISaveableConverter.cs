using System;
using System.Globalization;

namespace SimplySave {
    public interface ISaveableObjectConverter<TSaveable, TObject> where TSaveable : ISaveable, new() {

        TSaveable ConvertToSaveable(TObject obj);

        TObject ConvertFromSaveable(TSaveable saveable);

    }

    public interface ISaveableValueConverter<TValue, TObject> where TValue : IConvertible {

        TValue ConvertToValue(TObject obj);

        TObject ConvertFromValue(TValue value);

    }

    public class SaveableConverters {

        public static readonly ISaveableValueConverter<string, Guid> Guid = new SaveableValueConverter<string, Guid>.Inline(v => v.ToString("D", CultureInfo.InvariantCulture), System.Guid.Parse);
        public static readonly ISaveableValueConverter<string, TimeSpan> TimeSpan = new SaveableValueConverter<string, TimeSpan>.Inline(v => v.ToString(null, CultureInfo.InvariantCulture), v => System.TimeSpan.Parse(v, CultureInfo.InvariantCulture));

    }

    public abstract class SaveableObjectConverter<TSaveable, TObject> : ISaveableObjectConverter<TSaveable, TObject> where TSaveable : ISaveable, new() {

        public abstract TSaveable ConvertToSaveable(TObject obj);

        public abstract TObject ConvertFromSaveable(TSaveable saveable);

        TSaveable ISaveableObjectConverter<TSaveable, TObject>.ConvertToSaveable(TObject obj) {
            return this.ConvertToSaveable(obj);
        }

        TObject ISaveableObjectConverter<TSaveable, TObject>.ConvertFromSaveable(TSaveable saveable) {
            return this.ConvertFromSaveable((TSaveable) saveable);
        }

        public class Inline : SaveableObjectConverter<TSaveable, TObject> {

            private readonly Func<TObject, TSaveable> toSaveable;
            private readonly Func<TSaveable, TObject> fromSaveable;

            public Inline(Func<TObject, TSaveable> toSaveable, Func<TSaveable, TObject> fromSaveable) {
                this.toSaveable = toSaveable;
                this.fromSaveable = fromSaveable;
            }

            /// <inheritdoc />
            public override TSaveable ConvertToSaveable(TObject obj) {
                return this.toSaveable(obj);
            }

            /// <inheritdoc />
            public override TObject ConvertFromSaveable(TSaveable saveable) {
                return this.fromSaveable(saveable);
            }

        }

    }

    public abstract class SaveableValueConverter<TValue, TObject> : ISaveableValueConverter<TValue, TObject> where TValue : IConvertible {

        public abstract TValue ConvertToValue(TObject obj);

        public abstract TObject ConvertFromValue(TValue value);

        TValue ISaveableValueConverter<TValue, TObject>.ConvertToValue(TObject obj) {
            return this.ConvertToValue(obj);
        }

        TObject ISaveableValueConverter<TValue, TObject>.ConvertFromValue(TValue value) {
            return this.ConvertFromValue(value);
        }

        public class Inline : SaveableValueConverter<TValue, TObject> {

            private readonly Func<TObject, TValue> toValue;
            private readonly Func<TValue, TObject> fromValue;

            public Inline(Func<TObject, TValue> toValue, Func<TValue, TObject> fromValue) {
                this.toValue = toValue;
                this.fromValue = fromValue;
            }

            /// <inheritdoc />
            public override TValue ConvertToValue(TObject obj) {
                return this.toValue(obj);
            }

            /// <inheritdoc />
            public override TObject ConvertFromValue(TValue value) {
                return this.fromValue(value);
            }

        }

    }
}
