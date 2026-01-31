using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/manual", () => "Manual Route"); // Verify we can still mix

// Use Generated Router
AppRouter.Map(app);

app.Run();
