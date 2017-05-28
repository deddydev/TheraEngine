using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using CustomEngine.Files;
using System.IO;
using System.Xml;
using CustomEngine.Worlds.Actors;
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
