using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFredApps.TimeTracker.UI.Models
{
   public class SignUpViewModel : INotifyPropertyChanged
   {
      public bool isLoading { get; set; }

      public string? username { get; set; }
      public string? name { get; set; }
      public string? email { get; set; }
      public string? password { get; set; }
      public string? confirm_password { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;
   }
}
