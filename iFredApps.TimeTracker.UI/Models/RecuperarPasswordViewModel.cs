using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFredApps.TimeTracker.UI.Models
{

   public class RecuperarPasswordViewModel : INotifyPropertyChanged
   {
      public bool isLoading { get; set; }

      public string email { get; set; }
      public string codigo { get; set; }
      public string novaPassword { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;
   }
}
