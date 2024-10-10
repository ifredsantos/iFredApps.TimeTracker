namespace TimeTracker.UI.ViewModels
{
   public class LoginModel
   {
      public string user { get; set; }
      public string password { get; set; }
      public bool isLoading { get; set; }
   }

   public class LoginVM : ViewModelBase
   {
      private LoginModel login;
      //private string userSearchTerm;

      public LoginVM()
      {
         login = new LoginModel();
      }

      public string user
      {
         get => login.user;
         set
         {
            login.user = value;
            NotifyPropertyChanged(nameof(user));
         }
      }

      public string password
      {
         get => login.password;
         set
         {
            login.password = value;
            NotifyPropertyChanged(nameof(password));
         }
      }

      public bool isLoading
      {
         get => login.isLoading;
         set
         {
            login.isLoading = value;
            NotifyPropertyChanged(nameof(isLoading));
         }
      }
   }
}
