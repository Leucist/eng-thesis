namespace Application.AppMath
{
    public class LRUCache<TKey, TValue> where TKey : notnull
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, LinkedListNode<(TKey Key, TValue Value)>> _cacheMap;
        private readonly LinkedList<(TKey Key, TValue Value)> _lruList;

        public LRUCache(int capacity)
        {
            _capacity   = capacity;
            _cacheMap   = new Dictionary<TKey, LinkedListNode<(TKey, TValue)>>(capacity);
            _lruList    = new LinkedList<(TKey, TValue)>();
        }

        private void MoveToFirstInList(LinkedListNode<(TKey Key, TValue Value)> node) {
            _lruList.Remove(node);
            _lruList.AddFirst(node);
        }

        public TValue Get(TKey key)
        {
            if (_cacheMap.TryGetValue(key, out var node))
            {
                // Move the accessed node to the front of the list
                MoveToFirstInList(node);
                return node.Value.Value;
            }
            throw new KeyNotFoundException();
        }

        public void Add(TKey key, TValue value)
        {
            if (_cacheMap.TryGetValue(key, out var node))
            {
                // Update existing node
                MoveToFirstInList(node);
                node.Value = (key, value);
            }
            else
            {
                if (_cacheMap.Count >= _capacity)
                {
                    // Remove the least recently used item
                    var lruNode = _lruList.Last;
                    if (lruNode != null)
                    {
                        _cacheMap.Remove(lruNode.Value.Key);
                        _lruList.RemoveLast();
                    }
                }
                // Add new item
                var newNode = new LinkedListNode<(TKey, TValue)>((key, value));
                _lruList.AddFirst(newNode);
                _cacheMap[key] = newNode;
            }
        }
    }
}