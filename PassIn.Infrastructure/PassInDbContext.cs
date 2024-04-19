using Bogus;
using Microsoft.EntityFrameworkCore;
using PassIn.Infrastructure.Entities;
//dotnet ef migrations add CreateEventsTable --project PassIn.Infrastructure
//dotnet ef database update --project PassIn.Infrastructure
//dotnet ef migrations add CreateAttendeesTable --project PassIn.Infrastructure

namespace PassIn.Infrastructure;

public class PassInDbContext : DbContext
{
    //use .net secrets to get the connection string in program.cs
    // public PassInDbContext()
    // {
    //     
    // }
    // public PassInDbContext(DbContextOptions options) : base(options)
    // {
    //     
    // }
    public DbSet<Event> Events { get; set; }
    public DbSet<Attendee> Attendees { get; set; }
    public DbSet<CheckIn> CheckIns { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Environment.SetEnvironmentVariable("DB_HOST", "localhost");
        // Environment.SetEnvironmentVariable("DB_NAME", "PassInDb");
        // Environment.SetEnvironmentVariable("DB_SA_PASSWORD", "reallyStrongPwd123");
        
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME");
        var dbSaPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
        optionsBuilder.UseSqlServer(@$"Data Source={dbHost},1433;Initial Catalog={dbName};User Id=sa; Password={dbSaPassword};TrustServerCertificate=True;");
        //"Data Source=localhost,1433;Initial Catalog=PassInDb;User Id=sa; Password=reallyStrongPwd123;TrustServerCertificate=True;"
    }

    public static void SeedDatabase()
    {
        var dbContext = new PassInDbContext();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        
        Guid eventId = new Guid("9e9bd979-9d10-4915-b339-3786b1634f33");
        var eventEntity = new Event
        {
            Id = eventId,
            Title = "Unite Summit",
            Details = "Um evento para devs apaixonados por código!",
            MaximumAttendees = 120,
            Slug = "Unite Summit".ToLower().Replace(" ", "-")
        };
        dbContext.Events.Add(eventEntity);
        dbContext.SaveChanges();
        
        List<Attendee> attendees = new List<Attendee>();
        for (int i = 1; i <= 120; i++)
        {
            var faker = new Faker("pt_BR");
            var attendee = new Attendee
            {
                Name = faker.Name.FullName(), 
                Email = faker.Internet.Email().ToLower(), 
                EventId = new Guid(eventId.ToString()),
                CreatedAt = faker.Date.Recent(30),
                //65% chance of the attendee has made check-in
                CheckIn = faker.Random.Bool(0.65f) ? new CheckIn
                {
                    CreatedAt = faker.Date.Recent(5)
                } : null,
            };
            attendees.Add(attendee);
        }
        
        dbContext.Attendees.AddRange(attendees);
        dbContext.SaveChanges();
        
        Console.WriteLine("Database seeded!");
    }
}