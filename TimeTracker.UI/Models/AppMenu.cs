using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimeTracker.UI.Models
{
   public class AppMenu
   {
      public string description { get; set; }
      public FrameworkElement screen { get; set; }
      public PackIconFontAwesomeKind icon { get; set; }

      public AppMenu(string description, PackIconFontAwesomeKind icon)
      {
         this.description = description;
         this.icon = icon;
      }

      public AppMenu(string description, PackIconFontAwesomeKind icon, FrameworkElement screen)
      {
         this.description = description;
         this.icon = icon;
         this.screen = screen;
      }
   }
}
