using MahApps.Metro.IconPacks;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace iFredApps.TimeTracker.UI.Models
{
   public class AppMenuItem
   {
      public string description { get; set; }
      public FrameworkElement screen { get; set; }
      public PackIconFontAwesomeKind icon { get; set; }

      public AppMenuItem(string description, PackIconFontAwesomeKind icon)
      {
         this.description = description;
         this.icon = icon;
      }

      public AppMenuItem(string description, PackIconFontAwesomeKind icon, FrameworkElement screen)
      {
         this.description = description;
         this.icon = icon;
         this.screen = screen;
      }
   }

   public class AppMenu
   {
      public List<AppMenuItem> MenuList { get; set; }
      public User UserData { get; set; }

      public AppMenu()
      {
         MenuList = new List<AppMenuItem>();
      }
   }
}
