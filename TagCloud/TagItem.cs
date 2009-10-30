﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace TagCloud
{
    public class TagItem : INotifyPropertyChanged
    {
        private int _weight = 1;
        private bool _selected = false;
        private string _name;

        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                if ((this._name.Equals(value) != true))
                {
                    this._name = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return this._selected;
            }
            set
            {
                if ((this._selected.Equals(value) != true))
                {
                    this._selected = value;
                    this.RaisePropertyChanged("IsSelected");
                }
            }
        }

        public int Weighting
        {
            get
            {
                return this._weight;
            }
            set
            {
                if ((this._weight.Equals(value) != true))
                {
                    this._weight = value;
                    this.RaisePropertyChanged("Weighting");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
