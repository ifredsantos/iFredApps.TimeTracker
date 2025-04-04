﻿using System.ComponentModel;

namespace iFredApps.TimeTracker.UI.Models
{
   public class LoginViewModel : INotifyPropertyChanged
   {
      public bool isLoading { get; set; }

      public string user { get; set; }
      public string password { get; set; }
      public bool savePassword { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;
   }
}
