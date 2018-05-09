﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace System.Chase
{
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        private const string CountPropertyKey = "Count";
        private const string ItemIndexPropertyKey = "Item[]";

        public ObservableRangeCollection()
        {
        }

        public ObservableRangeCollection(IEnumerable<T> collection) : base(collection)
        {
        }

        /// <summary>
        ///     Adds a range of items to the existing list
        /// </summary>
        /// <param name="range">enumerable colection to add to the list</param>
        public void AddRange(IEnumerable<T> range)
        {
            var newItems = range.ToArray();
            for (var index = 0; index < newItems.Length; index++)
                Items.Add(newItems[index]);

            RaiseChanged(NotifyCollectionChangedAction.Reset);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanging;

        /// <summary>
        ///     Clears the existing list and replaces it with `range`
        /// </summary>
        /// <param name="range">enumerable collection to replace the items list with</param>
        public void Reset(IEnumerable<T> range)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, range, Items);
            RaiseCollectionChanging(args);
            Items.Clear();
            AddRange(range);
        }


        public void Update(IEnumerable<T> range) => Update(range, EqualityComparer<T>.Default);

        public void Update(IEnumerable<T> range, IEqualityComparer<T> comparer)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, range, Items);
            RaiseCollectionChanging(args);

            var newitems = range.ToArray();
            var itemsForRemoval = Items.Except(newitems, comparer).ToArray();
            var itemsForAddition = newitems.Except(Items, comparer).ToArray();

            for (var index = 0; index < itemsForAddition.Length; index++)
                Items.Add(itemsForAddition[index]);

            for (var index = 0; index < itemsForRemoval.Length; index++)
                Items.Remove(itemsForRemoval[index]);

            RaiseChanged(NotifyCollectionChangedAction.Reset);
        }

        private void RaiseChanged(NotifyCollectionChangedAction action)
        {
            try
            {
                OnPropertyChanged(new PropertyChangedEventArgs(CountPropertyKey));
                OnPropertyChanged(new PropertyChangedEventArgs(ItemIndexPropertyKey));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
            }
            catch
            {
                /* sometimes this fails, must dig into why */
            }
        }

        private void RaiseCollectionChanging(NotifyCollectionChangedEventArgs args) => CollectionChanging?.Invoke(this, args);
    }
}
