namespace iFredApps.TimeTracker.Core
{
   public class Result<T>
   {
      public bool Success { get; set; } = true;
      public T Data { get; set; }
      public List<string> Errors { get; set; }

      public static Result<T> Ok(T data) => new Result<T> { Success = true, Data = data };
      public static Result<T> Fail(params string[] errors) => new Result<T> { Success = false, Errors = errors.ToList() };
      public static Result<T> Fail(string error) => new Result<T> { Success = false, Errors = new List<string> { error } };
   }
}
