using System.Collections.Generic;
using System.Collections;
using TheraEngine.Worlds.Actors;
using Thera.Worlds.Actors.Items;
using TheraEngine.Worlds.Actors.Components;

namespace Thera.Worlds.Actors.Components.Logic
{
    public class InventoryComponent : LogicComponent, IEnumerable<InventoryItem>
    {
        public InventoryComponent()
        {
            _items = new EventList<InventoryItem>();
            _items.PostAdded += _items_PostAdded;
            _items.PostAddedRange += _items_PostAddedRange;
            _items.PostInserted += _items_PostInserted;
            _items.PostInsertedRange += _items_PostInsertedRange;
            _items.PostRemoved += _items_PostRemoved;
            _items.PostRemovedRange += _items_PostRemovedRange;
        }

        private void _items_PostRemovedRange(IEnumerable<InventoryItem> items)
        {

        }

        private void _items_PostRemoved(InventoryItem item)
        {

        }

        private void _items_PostInsertedRange(IEnumerable<InventoryItem> items, int index)
        {

        }

        private void _items_PostInserted(InventoryItem item, int index)
        {

        }

        private void _items_PostAddedRange(IEnumerable<InventoryItem> items)
        {

        }

        private void _items_PostAdded(InventoryItem item)
        {

        }

        EventList<InventoryItem> _items;

        public EventList<InventoryItem> Items => _items;

        public IEnumerator<InventoryItem> GetEnumerator() => ((IEnumerable<InventoryItem>)_items).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<InventoryItem>)_items).GetEnumerator();
    }
}
