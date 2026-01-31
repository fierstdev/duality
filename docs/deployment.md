# Deploying CSX Applications

Since CSX projects are standard ASP.NET Core applications, they can be hosted anywhere .NET 8 is supported.

## 1. Create a Release Build
Run the `publish` command to generate optimized production assets.

```bash
dotnet publish -c Release -o ./publish
```

This creates a `publish` directory containing everything needed to run your app.

## 2. Hosting Options

### Docker (Recommended)
Create a `Dockerfile` in your project root:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "YourProjectName.dll"]
```

Build and run:
```bash
docker build -t csx-app .
docker run -p 8080:8080 csx-app
```

### Azure App Service
1. Create a Web App in Azure Portal (Select Runtime stack: **.NET 8**).
2. Deploy using Azure CLI or VS Code.
   ```bash
   az webapp up --name <your-app-name> --resource-group <your-rg> --runtime "DOTNET|8.0"
   ```

### Linux VPS (Nginx)
1. Copy the contents of the `publish` folder to your server (e.g., `/var/www/csx-app`).
2. Run the app as a service using systemd.
   ```ini
   [Unit]
   Description=CSX App

   [Service]
   WorkingDirectory=/var/www/csx-app
   ExecStart=/usr/bin/dotnet /var/www/csx-app/YourProjectName.dll
   Restart=always
   User=www-data
   Environment=ASPNETCORE_ENVIRONMENT=Production

   [Install]
   WantedBy=multi-user.target
   ```

### Static Hosting?
CSX currently relies on server-side rendering (SSR), so it requires a .NET server. It cannot be deployed to static hosts like Vercel (unless using their Serverless Function support for .NET) or Netlify Drop.

## 3. Managing NuGet Versions

When publishing packages to [nuget.org](https://www.nuget.org), you might have old or broken versions (e.g., `1.0.5`) that you don't want users to install.

### Unlisting Packages
You cannot "delete" a package from NuGet (to prevent breaking builds), but you can **Unlist** it so it doesn't appear in search results or as the "Latest Version".

1. Go to [nuget.org/account/Packages](https://www.nuget.org/account/Packages).
2. Click the **Manage** (pencil icon) next to the package (e.g., `Duality.CLI`).
3. Select **Listing** in the sidebar.
4. Uncheck **List in search results**.
5. Click **Save**.

*Repeat this for `Duality.Core` and `Duality.Compiler` if needed.*

### Stable vs Prerelease
- **Stable** (e.g., `1.0.0`): The default installation target.
- **Prerelease** (e.g., `1.1.4-alpha`): Requires the `--prerelease` flag to install.

To ensure users get your latest alpha/beta work:
```bash
dotnet tool install -g Duality.CLI --prerelease
```
