# Use the official Microsoft ASP.NET Core runtime image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["HamperStore.Web/HamperStore.Web.csproj", "HamperStore.Web/"]
COPY ["HamperStore.Infrastructure/HamperStore.Infrastructure.csproj", "HamperStore.Infrastructure/"]
COPY ["HamperStore.Core/HamperStore.Core.csproj", "HamperStore.Core/"]
RUN dotnet restore "HamperStore.Web/HamperStore.Web.csproj"
COPY . .
WORKDIR "/src/HamperStore.Web"
RUN dotnet build "HamperStore.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HamperStore.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build the final image using base
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Ensure the uploads directory exists inside wwwroot
RUN mkdir -p wwwroot/images/hampers/uploads
# Set ASPNETCORE_URLS to bind to 8080 (standard for Render, Fly.io, Railway)
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "HamperStore.Web.dll"]
