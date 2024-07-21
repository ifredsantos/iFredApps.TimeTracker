namespace TimeTracker.UI.Models
{
    public class AppConfig
    {
        public enDataBaseType database_type { get; set; }
        public JSONDataBaseConfig json_database_config { get; set; }

        public enum enDataBaseType
        {
            JSON
        }
    }

    public class JSONDataBaseConfig
    {
        public string directory { get; set; }
        public string filename { get; set; }
    }
}
