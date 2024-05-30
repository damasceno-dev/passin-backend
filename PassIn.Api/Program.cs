
using dotenv.net;
using PassIn.Api.Filters;
using PassIn.Application.UseCases.Attendees.GetAllByEventId;
using PassIn.Application.UseCases.Checkins.DoCheckin;
using PassIn.Application.UseCases.Events.GetById;
using PassIn.Application.UseCases.Events.Register;
using PassIn.Application.UseCases.Events.RegisterAttendee;
using PassIn.Infrastructure;

string root = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
string envPath = Path.Combine(root, ".env");

DotEnv.Load(new DotEnvOptions(envFilePaths: new[] { envPath }));

// Console.WriteLine($"DB_HOST: {Environment.GetEnvironmentVariable("DB_HOST")}");
// Console.WriteLine($"DB_NAME: {Environment.GetEnvironmentVariable("DB_NAME")}");
// Console.WriteLine($"DB_SA_PASSWORD: {Environment.GetEnvironmentVariable("DB_SA_PASSWORD")}");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks(); // Add health checks

//redirects any exception from the project to the class ExceptionFilter
builder.Services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)));
// builder.Services.AddDbContext<PassInDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("PassIn")));

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Register application use cases and logging services
builder.Services.AddLogging();
builder.Services.AddScoped<RegisterEventUseCase>();
builder.Services.AddScoped<GetEventByIdUseCase>();
builder.Services.AddScoped<RegisterAttendeeOnEventUseCase>();
builder.Services.AddScoped<GetAllAttendeesByEventIdUseCase>();
builder.Services.AddScoped<DoAttendeeCheckInUseCase>();

if (Array.Exists(args, arg => arg.Equals("seed", StringComparison.CurrentCultureIgnoreCase)))
{
    PassInDbContext.SeedDatabase();
    return;
}

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
     // app.UseSwagger();
     // app.UseSwaggerUI();
// }

app.UseSwagger();
app.UseSwaggerUI();
// app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

// Map health checks endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
