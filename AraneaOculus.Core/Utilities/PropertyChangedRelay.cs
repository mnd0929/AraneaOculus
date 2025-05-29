using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace AraneaOculus.Core.Utilities
{
    public class PropertyChangedRelay : IDisposable
    {
        private readonly INotifyPropertyChanged Root;

        private readonly PropertyChangedEventHandler Handler;

        private readonly HashSet<object> Visited = new();

        private readonly Dictionary<INotifyPropertyChanged, List<PropertyInfo>> Subscriptions = new();

        public PropertyChangedRelay(INotifyPropertyChanged root, PropertyChangedEventHandler handler)
        {
            Root = root;
            Handler = handler;
            SubscribeRecursive(Root);
        }

        private void SubscribeRecursive(INotifyPropertyChanged obj)
        {
            if (obj == null || Visited.Contains(obj))
                return;

            obj.PropertyChanged += OnAnyPropertyChanged!;
            Visited.Add(obj);

            var props = new List<PropertyInfo>();
            foreach (var prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead) continue;
                var value = prop.GetValue(obj);

                if (value is INotifyPropertyChanged npc && value != obj)
                {
                    SubscribeRecursive(npc);
                    props.Add(prop);
                }
                else if (value is IEnumerable enumerable && !(value is string))
                {
                    foreach (var item in enumerable)
                    {
                        if (item is INotifyPropertyChanged child)
                            SubscribeRecursive(child);
                    }
                }
            }
            Subscriptions[obj] = props;
        }

        private void OnAnyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Handler(sender, e);

            if (sender is INotifyPropertyChanged npc && Subscriptions.TryGetValue(npc, out var props))
            {
                foreach (var prop in props)
                {
                    if (prop.Name == e.PropertyName)
                    {
                        var value = prop.GetValue(npc);
                        if (value is INotifyPropertyChanged child)
                            SubscribeRecursive(child);
                        else if (value is IEnumerable enumerable && !(value is string))
                        {
                            foreach (var item in enumerable)
                            {
                                if (item is INotifyPropertyChanged childItem)
                                    SubscribeRecursive(childItem);
                            }
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (var obj in Visited)
            {
                if (obj is INotifyPropertyChanged npc)
                    npc.PropertyChanged -= OnAnyPropertyChanged!;
            }
            Visited.Clear();
            Subscriptions.Clear();
        }
    }
}
