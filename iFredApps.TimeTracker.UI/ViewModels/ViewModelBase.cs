﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace iFredApps.TimeTracker.UI.ViewModels
{
   public class ViewModelBase : INotifyPropertyChanged
   {

      public event PropertyChangedEventHandler PropertyChanged;
      protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}
