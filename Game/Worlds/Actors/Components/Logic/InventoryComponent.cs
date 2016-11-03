using CustomEngine.Worlds.Actors.Components;
using Game.Worlds.Actors.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Game.Worlds.Actors.Components.Logic
{
    public class InventoryComponent : LogicComponent, IEnumerable<InventoryItem>
    {
        public MonitoredList<InventoryItem> Items { get { return _items; } }
        MonitoredList<InventoryItem> _items = new MonitoredList<InventoryItem>();

        public IEnumerator<InventoryItem> GetEnumerator() { return ((IEnumerable<InventoryItem>)_items).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<InventoryItem>)_items).GetEnumerator(); }
    }
}
