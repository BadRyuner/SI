﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SIUI.ViewModel
{
    public abstract class ViewModelBase<T> : INotifyPropertyChanged
        where T : new()
    {
        protected T _model;

        public T Model => _model;

        protected ViewModelBase()
        {
            _model = new T();
        }

        protected ViewModelBase(T model)
        {
            _model = model;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
