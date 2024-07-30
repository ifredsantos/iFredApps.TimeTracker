namespace TimeTracker.SL
{
   public class sSession
   {
      public int session_id { get; set; }
      public DateTime start_date { get; set; }
      public DateTime? end_date { get; set; }
      public string? description { get; set; }
      public string? observation { get; set; }
   }
}
