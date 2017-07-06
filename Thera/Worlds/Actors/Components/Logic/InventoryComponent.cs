using System.Collections.Generic;
using System.Collections;
using TheraEngine.Worlds.Actors;
using Thera.Worlds.Actors.Items;

namespace Thera.Worlds.Actors.Components.Logic
{
    public class InventoryComponent : LogicComponent, IEnumerable<InventoryItem>
    {
        MonitoredList<InventoryItem> _items = new MonitoredList<InventoryItem>();

        public MonitoredList<InventoryItem> Items => _items;

        public IEnumerator<InventoryItem> GetEnumerator() => ((IEnumerable<InventoryItem>)_items).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<InventoryItem>)_items).GetEnumerator();
    }
}
