using PassIn.Api.Filters;
using PassIn.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

app.MapControllers();

app.Run();
