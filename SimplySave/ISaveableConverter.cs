using System;
using System.Globalization;

namespace SimplySave {
    public interface ISaveableConverter<T> {

        ISaveable CreateSaveable(string type);

        ISaveable ConvertToSaveable(T obj);

        T ConvertFromSaveable(ISaveable saveable);

        IConvertible ConvertToValue(T obj);

        T ConvertFromValue(IConvertible value);

    }

    public class SaveableConverters {

        public static readonly ISaveableConverter<Guid> Guid = new ValueSaveableConverter<string, Guid>.Inline(v => v.ToString("D", CultureInfo.InvariantCulture), System.Guid.Parse);
        public static readonly ISaveableConverter<TimeSpan> TimeSpan = new ValueSaveableConverter<string, TimeSpan>.Inline(v => v.ToString(null, CultureInfo.InvariantCulture), v => System.TimeSpan.Parse(v, CultureInfo.InvariantCulture));

    }

    public abstract class ObjectSaveableConverter<TSaveable, TObject> : ISaveableConverter<TObject> where TSaveable : ISaveable {

        public abstract TSaveable CreateSaveable(string type);

        public abstract TSaveable ConvertToSaveable(TObject obj);

        public abstract TObject ConvertFromSaveable(TSaveable saveable);

        ISaveable ISaveableConverter<TObject>.CreateSaveable(string type) {
            return this.CreateSaveable(type);
        }

        ISaveable ISaveableConverter<TObject>.ConvertToSaveable(TObject obj) {
            return this.ConvertToSaveable(obj);
        }

        TObject ISaveableConverter<TObject>.ConvertFromSaveable(ISaveable saveable) {
            return this.ConvertFromSaveable((TSaveable) saveable);
        }

        IConvertible ISaveableConverter<TObject>.ConvertToValue(TObject obj) {
            throw new NotSupportedException("ObjectSaveableConverter can only convert to and from saveables, not values");
        }

        TObject ISaveableConverter<TObject>.ConvertFromValue(IConvertible value) {
            throw new NotSupportedException("ObjectSaveableConverter can only convert to and from saveables, not values");
        }

        public class Inline : ObjectSaveableConverter<TSaveable, TObject> {

            private readonly Func<string, TSaveable> createSaveable;
            private readonly Func<TObject, TSaveable> toSaveable;
            private readonly Func<TSaveable, TObject> fromSaveable;

            public Inline(Func<string, TSaveable> createSaveable, Func<TObject, TSaveable> toSaveable, Func<TSaveable, TObject> fromSaveable) {
                this.createSaveable = createSaveable;
                this.toSaveable = toSaveable;
                this.fromSaveable = fromSaveable;
            }

            /// <inheritdoc />
            public override TSaveable CreateSaveable(string type) {
                return this.createSaveable(type);
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

    public abstract class ValueSaveableConverter<TValue, TObject> : ISaveableConverter<TObject> where TValue : IConvertible {

        public abstract TValue ConvertToValue(TObject obj);

        public abstract TObject ConvertFromValue(TValue value);

        IConvertible ISaveableConverter<TObject>.ConvertToValue(TObject obj) {
            return this.ConvertToValue((TObject) obj);
        }

        TObject ISaveableConverter<TObject>.ConvertFromValue(IConvertible value) {
            return this.ConvertFromValue((TValue) value);
        }

        ISaveable ISaveableConverter<TObject>.CreateSaveable(string type) {
            throw new NotSupportedException("ValueSaveableConverter can only convert to and from values, not saveables");
        }

        ISaveable ISaveableConverter<TObject>.ConvertToSaveable(TObject obj) {
            throw new NotSupportedException("ValueSaveableConverter can only convert to and from values, not saveables");
        }

        TObject ISaveableConverter<TObject>.ConvertFromSaveable(ISaveable saveable) {
            throw new NotSupportedException("ValueSaveableConverter can only convert to and from values, not saveables");
        }

        public class Inline : ValueSaveableConverter<TValue, TObject> {

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
