var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles(); // Serve wwwroot/app.css

// Register CSX Routes
AppRouter.Map(app);

app.Run();
