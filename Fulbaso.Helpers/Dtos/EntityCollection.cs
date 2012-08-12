using System;
using System.Collections;
using System.Collections.Generic;

namespace Fulbaso.Helpers
{
    [Serializable]
    public class EntityCollection<T> : IEnumerable<T>, IComparable, IFormattable
    {
        private IEnumerable<T> _collection;

        public EntityCollection(IEnumerable<T> collection)
        {
            _collection = collection;
        }

        public static implicit operator EntityCollection<T>(List<T> collection)
        {
            return collection == null ? null : new EntityCollection<T>(collection);
        }

        public override string ToString()
        {
            if (_collection == null) return null;

            return this.ToString(" / ");
        }

        public string ToString(string separator)
        {
            if (_collection == null) return null;

            // creates a string array
            string result = string.Empty;
            foreach (var obj in _collection) result += obj.ToString() + separator;

            return string.IsNullOrEmpty(result) ? string.Empty : result.Substring(0, result.Length - separator.Length);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.ToString(format);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_collection == null) return null;
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_collection == null) return null;
            return _collection.GetEnumerator();
        }

        public int CompareTo(object obj)
        {
            return this.ToString().CompareTo(obj.ToString());
        }
    }
}
