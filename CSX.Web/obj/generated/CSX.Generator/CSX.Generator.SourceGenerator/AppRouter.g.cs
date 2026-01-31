using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text;

public static class AppRouter
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/app.css", () => Results.Text(CssBundle.AllCss, "text/css"));
        app.MapGet("/about", (HttpContext ctx) =>
        {
            var sb = new StringBuilder();
            Layout_Impl.Render(sb, null, (s) => About_Impl.Render(s));
            return Results.Content(sb.ToString(), "text/html");
        });
        app.MapGet("/", (HttpContext ctx) =>
        {
            var sb = new StringBuilder();
            Layout_Impl.Render(sb, null, (s) => Index_Impl.Render(s));
            return Results.Content(sb.ToString(), "text/html");
        });
        app.MapPost("/_rpc/Index/Save", async (HttpRequest req) =>
        {
            var args = await req.ReadFromJsonAsync<System.Text.Json.JsonElement[]>();
            if (args == null || args.Length != 1) return Results.BadRequest("Invalid arguments");
            var arg0 = System.Text.Json.JsonSerializer.Deserialize<string>(args[0]);
            Index_Impl.Save(arg0);
            return Results.Ok();
        });
        app.MapGet("/rpctest", (HttpContext ctx) =>
        {
            var sb = new StringBuilder();
            Layout_Impl.Render(sb, null, (s) => RpcTest_Impl.Render(s));
            return Results.Content(sb.ToString(), "text/html");
        });
        app.MapPost("/_rpc/RpcTest/MyAction", async (HttpRequest req) =>
        {
            var args = await req.ReadFromJsonAsync<System.Text.Json.JsonElement[]>();
            if (args == null || args.Length != 1) return Results.BadRequest("Invalid arguments");
            var arg0 = System.Text.Json.JsonSerializer.Deserialize<string>(args[0]);
            RpcTest_Impl.MyAction(arg0);
            return Results.Ok();
        });
        app.MapGet("/styletest", (HttpContext ctx) =>
        {
            var sb = new StringBuilder();
            Layout_Impl.Render(sb, null, (s) => StyleTest_Impl.Render(s));
            return Results.Content(sb.ToString(), "text/html");
        });
        app.MapGet("/user/{id}", (HttpContext ctx) =>
        {
            var sb = new StringBuilder();
            var routeParams = new System.Collections.Generic.Dictionary<string, object>();
            foreach(var kvp in ctx.Request.RouteValues) { routeParams[kvp.Key] = kvp.Value; }
            Layout_Impl.Render(sb, routeParams, (s) => UserProfile_Impl.Render(s, routeParams));
            return Results.Content(sb.ToString(), "text/html");
        });
    }
}
