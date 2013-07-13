using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace System.Collections.ObjectModel
{
    [Serializable]
    public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly List<T> _wrapped;

        public ObservableCollection()
        {
            _wrapped = new List<T>();
        }

        public ObservableCollection(IEnumerable<T> collection)
        {
            _wrapped = new List<T>(collection);
        }

        public ObservableCollection(List<T> list)
        {
            _wrapped = new List<T>(list);
        }
    }
}
