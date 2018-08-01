using System.Collections;

namespace System.Diagnostics
{
    /// <summary>Provides a strongly typed collection of <see cref="T:System.Diagnostics.CounterCreationData" /> objects.</summary>
    /// <filterpriority>2</filterpriority>
    [Serializable]
    public class CounterCreationDataCollection : CollectionBase
    {
        /// <summary>Indexes the <see cref="T:System.Diagnostics.CounterCreationData" /> collection.</summary>
        /// <returns>The collection index, which is used to access individual elements of the collection.</returns>
        /// <param name="index">An index into the <see cref="T:System.Diagnostics.CounterCreationDataCollection" />. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index" /> is less than 0.-or-<paramref name="index" /> is equal to or greater than the number of items in the collection.</exception>
        /// <filterpriority>2</filterpriority>
        public CounterCreationData this[int index]
        {
            get => (CounterCreationData)List[index];
            set => List[index] = value;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.CounterCreationDataCollection" /> class, with no associated <see cref="T:System.Diagnostics.CounterCreationData" /> instances.</summary>
        public CounterCreationDataCollection()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.CounterCreationDataCollection" /> class by using the specified collection of <see cref="T:System.Diagnostics.CounterCreationData" /> instances.</summary>
        /// <param name="value">A <see cref="T:System.Diagnostics.CounterCreationDataCollection" /> that holds <see cref="T:System.Diagnostics.CounterCreationData" /> instances with which to initialize this <see cref="T:System.Diagnostics.CounterCreationDataCollection" />. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is null.</exception>
        public CounterCreationDataCollection(CounterCreationDataCollection value)
        {
            AddRange(value);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.CounterCreationDataCollection" /> class by using the specified array of <see cref="T:System.Diagnostics.CounterCreationData" /> instances.</summary>
        /// <param name="value">An array of <see cref="T:System.Diagnostics.CounterCreationData" /> instances with which to initialize this <see cref="T:System.Diagnostics.CounterCreationDataCollection" />. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is null.</exception>
        public CounterCreationDataCollection(CounterCreationData[] value)
        {
            AddRange(value);
        }

        /// <summary>Adds an instance of the <see cref="T:System.Diagnostics.CounterCreationData" /> class to the collection.</summary>
        /// <returns>The index of the new <see cref="T:System.Diagnostics.CounterCreationData" /> object.</returns>
        /// <param name="value">A <see cref="T:System.Diagnostics.CounterCreationData" /> object to append to the existing collection. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="value" /> is not a <see cref="T:System.Diagnostics.CounterCreationData" /> object.</exception>
        /// <filterpriority>2</filterpriority>
        public int Add(CounterCreationData value)
        {
            return List.Add(value);
        }

        /// <summary>Adds the specified array of <see cref="T:System.Diagnostics.CounterCreationData" /> instances to the collection.</summary>
        /// <param name="value">An array of <see cref="T:System.Diagnostics.CounterCreationData" /> instances to append to the existing collection. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is null.</exception>
        /// <filterpriority>2</filterpriority>
        public void AddRange(CounterCreationData[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            for (int i = 0; i < value.Length; i++)
            {
                Add(value[i]);
            }
        }

        /// <summary>Adds the specified collection of <see cref="T:System.Diagnostics.CounterCreationData" /> instances to the collection.</summary>
        /// <param name="value">A collection of <see cref="T:System.Diagnostics.CounterCreationData" /> instances to append to the existing collection. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is null.</exception>
        /// <filterpriority>2</filterpriority>
        public void AddRange(CounterCreationDataCollection value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            int count = value.Count;
            for (int i = 0; i < count; i++)
            {
                Add(value[i]);
            }
        }

        /// <summary>Determines whether a <see cref="T:System.Diagnostics.CounterCreationData" /> instance exists in the collection.</summary>
        /// <returns>true if the specified <see cref="T:System.Diagnostics.CounterCreationData" /> object exists in the collection; otherwise, false.</returns>
        /// <param name="value">The <see cref="T:System.Diagnostics.CounterCreationData" /> object to find in the collection. </param>
        /// <filterpriority>2</filterpriority>
        public bool Contains(CounterCreationData value)
        {
            return List.Contains(value);
        }

        /// <summary>Copies the elements of the <see cref="T:System.Diagnostics.CounterCreationData" /> to an array, starting at the specified index of the array.</summary>
        /// <param name="array">An array of <see cref="T:System.Diagnostics.CounterCreationData" /> instances to add to the collection. </param>
        /// <param name="index">The location at which to add the new instances. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the <see cref="T:System.Diagnostics.CounterCreationDataCollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination array.</exception>
        /// <filterpriority>2</filterpriority>
        public void CopyTo(CounterCreationData[] array, int index)
        {
            List.CopyTo(array, index);
        }

        /// <summary>Returns the index of a <see cref="T:System.Diagnostics.CounterCreationData" /> object in the collection.</summary>
        /// <returns>The zero-based index of the specified <see cref="T:System.Diagnostics.CounterCreationData" />, if it is found, in the collection; otherwise, -1.</returns>
        /// <param name="value">The <see cref="T:System.Diagnostics.CounterCreationData" /> object to locate in the collection. </param>
        /// <filterpriority>2</filterpriority>
        public int IndexOf(CounterCreationData value)
        {
            return List.IndexOf(value);
        }

        /// <summary>Inserts a <see cref="T:System.Diagnostics.CounterCreationData" /> object into the collection, at the specified index.</summary>
        /// <param name="index">The zero-based index of the location at which the <see cref="T:System.Diagnostics.CounterCreationData" /> is to be inserted. </param>
        /// <param name="value">The <see cref="T:System.Diagnostics.CounterCreationData" /> to insert into the collection. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="value" /> is not a <see cref="T:System.Diagnostics.CounterCreationData" /> object.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index" /> is less than 0. -or-<paramref name="index" /> is greater than the number of items in the collection.</exception>
        /// <filterpriority>2</filterpriority>
        public void Insert(int index, CounterCreationData value)
        {
            List.Insert(index, value);
        }

        /// <summary>Removes a <see cref="T:System.Diagnostics.CounterCreationData" /> object from the collection.</summary>
        /// <param name="value">The <see cref="T:System.Diagnostics.CounterCreationData" /> to remove from the collection. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="value" /> is not a <see cref="T:System.Diagnostics.CounterCreationData" /> object.-or-<paramref name="value" /> does not exist in the collection.</exception>
        /// <filterpriority>2</filterpriority>
        public virtual void Remove(CounterCreationData value)
        {
            List.Remove(value);
        }

        /// <summary>Checks the specified object to determine whether it is a valid <see cref="T:System.Diagnostics.CounterCreationData" /> type.</summary>
        /// <param name="value">The object that will be validated.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="value" /> is not a <see cref="T:System.Diagnostics.CounterCreationData" /> object.</exception>
        protected override void OnValidate(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            CounterCreationData counterCreationData = value as CounterCreationData;
            if (counterCreationData == null)
            {
                throw new ArgumentException("MustAddCounterCreationData");
            }
        }
    }
}