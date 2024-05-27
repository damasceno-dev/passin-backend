using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PassIn.Infrastructure.Entities;

namespace PassIn.Infrastructure;

public class PassInDbContext : DbContext
{
    public DbSet<Event> Events { get; set; }
    public DbSet<Attendee> Attendees { get; set; }
    public DbSet<CheckIn> CheckIns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var isDocker = File.Exists("/.dockerenv");
        var dbHost = isDocker ? Environment.GetEnvironmentVariable("DB_HOST") : "localhost";
        var dbName = isDocker ? Environment.GetEnvironmentVariable("DB_NAME") : "PassInDb";
        var dbSaPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");

        var connectionString = @$"Data Source={dbHost},1433;Initial Catalog={dbName};User Id=sa; Password={dbSaPassword};TrustServerCertificate=True;";
        Console.WriteLine($"Using Connection String: {connectionString}");

        optionsBuilder.UseSqlServer(connectionString);
    }

    public static void SeedDatabase()
    {
        var optionsBuilder = new DbContextOptionsBuilder<PassInDbContext>();
        var context = new PassInDbContext();
        context.OnConfiguring(optionsBuilder);

        using var dbContext = new PassInDbContext();
        dbContext.Database.Migrate();

        Guid eventId = new Guid("9e9bd979-9d10-4915-b339-3786b1634f33");
        var existingSeedingEvent = dbContext.Events.FirstOrDefault(e => e.Id == eventId);

        if (existingSeedingEvent == null)
        {
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
        else
        {
            Console.WriteLine("Seeding Event already exists in the database. Seed was not applied.");
        }
    }
}
