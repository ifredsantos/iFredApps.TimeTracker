namespace iFredApps.TimeTracker.Tests
{
   //public class ImportData : IClassFixture<DatabaseTestFixture>
   //{
   //   private readonly AppDbContext _context;
   //   private readonly SessionService _sessionService;

   //   public ImportData(DatabaseTestFixture fixture)
   //   {
   //      _context = fixture.Context;
   //      _sessionService = new SessionService(_context);
   //   }


   //   [Fact]
   //   public async void ImportSession()
   //   {
   //      try
   //      {
   //         var users = await _sessionService.GetUserSessions(2);
   //         if (users != null)
   //         {
   //            foreach (var user in users)
   //            {
   //            }
   //         }


   //         var result = await _sessionService.CreateSession(new Session
   //         {
   //            user_id = 2,
   //            start_date = new DateTime(2024, 10, 31, 9, 0, 0),
   //            end_date = new DateTime(2024, 10, 31, 12, 0, 0),
   //            description = "Teste"
   //         });
   //      }
   //      catch (Exception ex)
   //      {

   //         throw;
   //      }
   //   }
   //}

   //public class DatabaseTestFixture : IDisposable
   //{
   //   public AppDbContext Context { get; private set; }

   //   public DatabaseTestFixture()
   //   {
   //      // Configura o DbContext para usar um banco MySQL real
   //      var options = new DbContextOptionsBuilder<AppDbContext>()
   //          .UseMySql("Server=localhost;Database=TestDatabase;User=root;Password=yourpassword;",
   //                    new MySqlServerVersion(new Version(8, 0, 23)))
   //          .Options;

   //      Context = new AppDbContext(options);

   //      // Criar o banco de dados e aplicar migrações
   //      Context.Database.EnsureCreated();
   //   }

   //   public void Dispose()
   //   {
   //      // Limpeza: apaga o banco após os testes
   //      Context.Database.EnsureDeleted();
   //      Context.Dispose();
   //   }
   //}
}