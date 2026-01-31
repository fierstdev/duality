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
