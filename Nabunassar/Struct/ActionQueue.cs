using System.Collections;

namespace Nabunassar.Struct
{
    internal class ActionQueue
    {
        private List<Action> _queue { get; set; } = new();

        public void Enqueue(Action action)
        {
            _queue.Add(action);
        }

        public Action Dequeue()
        {
            if (_queue.Count == 0)
                return default;

            var item = _queue[0];
            _queue.Remove(item);
            return item;
        }
    }
}
