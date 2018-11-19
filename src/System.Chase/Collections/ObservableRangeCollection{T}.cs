using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace System.Chase.Collections
{
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        private const string _countPropertyKey = "Count";
        private const string _itemIndexPropertyKey = "Item[]";

        public ObservableRangeCollection() { }
        
        public ObservableRangeCollection(IEnumerable<T> collection) : base(collection) { }

        /// <summary>
        ///     Adds a range of items to the existing list
        /// </summary>
        /// <param name="range">enumerable collection to add to the list</param>
        public void AddRange(IEnumerable<T> range)
        {
            var newItems = range.ToArray();
            for (var index = 0; index < newItems.Length; index++)
                Items.Add(newItems[index]);

            RaiseChanged(NotifyCollectionChangedAction.Reset);
        }

        /// <summary>
        ///     Clears the existing list and replaces it with `range`
        /// </summary>
        /// <param name="range">enumerable collection to replace the items list with</param>
        public void Reset(IEnumerable<T> range)
        {
            Items.Clear();
            AddRange(range);
        }

        /// <summary>
        /// Diffs the new range with the existing Items adding the new, removing the old, and leaving the matching in tact. Uses a <see cref="EqualityComparer{T}.Default"/> comparer
        /// </summary>
        /// <param name="range">range to diff with</param>
        public void Update(IEnumerable<T> range) => Update(range, EqualityComparer<T>.Default);

        /// <summary>
        /// Diffs the new range with the existing Items adding the new, removing the old, and leaving the matching in tact.ÏÏ
        /// </summary>
        /// <param name="range">range to diff with</param>
        /// <param name="comparer">Equality Comparer used to compare the items</param>
        public void Update(IEnumerable<T> range, IEqualityComparer<T> comparer)
        {
            var newItems = range.ToArray();
            var itemsForAddition = newItems.Except(Items, comparer).ToArray();
            var itemsForRemoval = Items.Except(newItems, comparer).ToArray();

            for (var index = 0; index < itemsForAddition.Length; index++)
                Items.Add(itemsForAddition[index]);

            for (var index = 0; index < itemsForRemoval.Length; index++)
                Items.Remove(itemsForRemoval[index]);

            RaiseChanged(NotifyCollectionChangedAction.Reset);
        }

        private void RaiseChanged(NotifyCollectionChangedAction action)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(_countPropertyKey));
            OnPropertyChanged(new PropertyChangedEventArgs(_itemIndexPropertyKey));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
        }
    }
}
