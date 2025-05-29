using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AraneaOculus.Core.Utilities
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged
    {
        private readonly Dictionary<TKey, TValue> _dictionary = new();
        private readonly ObservableCollection<TValue> _collection = new();

        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add
            {
                _collection.CollectionChanged += value;
                _externalHandlers += value;
            }
            remove
            {
                _collection.CollectionChanged -= value;
                _externalHandlers -= value;
            }
        }

        private event NotifyCollectionChangedEventHandler? _externalHandlers;

        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _collection;
        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                if (_dictionary.ContainsKey(key))
                {
                    UnsubscribeFromCollection(_dictionary[key]);
                    int idx = _collection.IndexOf(_dictionary[key]);
                    _collection[idx] = value;
                    _dictionary[key] = value;
                    SubscribeToCollection(value);
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            _collection.Add(value);
            SubscribeToCollection(key);
            SubscribeToCollection(value);
        }

        public bool Remove(TKey key)
        {
            if (_dictionary.TryGetValue(key, out var value))
            {
                _collection.Remove(value);
                UnsubscribeFromCollection(key);
                UnsubscribeFromCollection(value);
                return _dictionary.Remove(key);
            }
            return false;
        }

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            foreach (var key in _dictionary.Keys.ToList())
                UnsubscribeFromCollection(key);
            foreach (var value in _dictionary.Values.ToList())
                UnsubscribeFromCollection(value);

            _dictionary.Clear();
            _collection.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.ContainsKey(item.Key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var kvp in _dictionary)
                array[arrayIndex++] = kvp;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Подписка на CollectionChanged для ObservableCollection в ключе или значении
        private void SubscribeToCollection(object? obj)
        {
            if (obj is INotifyCollectionChanged collection)
                collection.CollectionChanged += OnInnerCollectionChanged;
        }

        private void UnsubscribeFromCollection(object? obj)
        {
            if (obj is INotifyCollectionChanged collection)
                collection.CollectionChanged -= OnInnerCollectionChanged;
        }

        // Проксируем внутренние события наружу
        private void OnInnerCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _externalHandlers?.Invoke(sender, e);
        }
    }
}