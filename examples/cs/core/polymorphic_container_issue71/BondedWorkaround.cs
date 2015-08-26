using Bond;

namespace generics
{
    /// <summary>
    /// Implementation of <see cref="IBonded"/> holding an instance of <typeparamref name="T"/>
    /// </summary>
    /// <remarks>
    /// This type is designed to be used in place of <see cref="Bond.Bonded{T}"/> in Bond.CSharp.3.0.6
    /// to fix github issue https://github.com/Microsoft/bond/issues/71. It will be deprecated as soon
    /// as a proper fix is available.
    /// </remarks>
    /// <typeparam name="T">Type representing a Bond schema</typeparam>
    public sealed class BondedWorkaround<T> : IBonded<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BondedWorkaround{T}"/> class
        /// </summary>
        /// <param name="value">Object of type <typeparamref name="T"/></param>
        public BondedWorkaround(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the bonded value
        /// </summary>
        public T Value { get; private set; }

        T IBonded<T>.Deserialize()
        {
            return Clone<T>.From(this.Value);
        }

        U IBonded.Deserialize<U>()
        {
            if (this.Value is U)
            {
                return (U)(object)Clone<T>.From(this.Value);
            }
            return Clone<U>.From(this.Value as U);
        }

        void IBonded.Serialize<W>(W writer)
        {
            Serialize.To(writer, this.Value);
        }

        IBonded<U> IBonded.Convert<U>()
        {
            if (this.Value is U)
            {
                return (IBonded<U>)(IBonded)new BondedWorkaround<T>(this.Value);
            }
            return new BondedWorkaround<U>(this.Value as U);
        }
    }
}
