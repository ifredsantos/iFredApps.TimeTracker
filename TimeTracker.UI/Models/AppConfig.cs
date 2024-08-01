namespace TimeTracker.UI.Models
{
    public class AppConfig
    {
        public enDataBaseType database_type { get; set; }
        public JSONDataBaseConfig json_database_config { get; set; }
        public WebApiConnectionConfig webapi_connection_config { get; set; }

        public enum enDataBaseType
        {
            JSON,
            WebApi
        }
   }

   public class JSONDataBaseConfig
   {
      public string directory { get; set; }
      public string filename { get; set; }
   }

   public class WebApiConnectionConfig
   {
      public string baseaddress { get; set; }
   }
}
