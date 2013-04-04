using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core;

namespace Models
{
    public class ModelBase : INotifyPropertyChanged, IPropertyChangeTracker
    {
        private readonly Dictionary<string, bool> _propertyDirtyHash = new Dictionary<string, bool>();

        public ModelBase()
        {
            PropertyChanged += (sender, args) =>
            {
                _propertyDirtyHash[args.PropertyName] = true;
            };
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public bool IsModified(string name)
        {
            return _propertyDirtyHash.ContainsKey(name);
        }
    }
}