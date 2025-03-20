# syntax=docker/dockerfile:1

# Define the .NET version to use
ARG DOTNET_VERSION=8.0

# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-bookworm-slim AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image for compiling the application
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-bookworm-slim AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["RazorApp/Dove.Blog.WebApp.csproj", "RazorApp/"]
COPY ["Dove.Blog.Abstractions/Dove.Blog.Abstractions.csproj", "Dove.Blog.Abstractions/"]
COPY ["Dove.Blog.Data/Dove.Blog.Data.csproj", "Dove.Blog.Data/"]
COPY ["Dove.Blog.Logic/Dove.Blog.Logic.csproj", "Dove.Blog.Logic/"]
RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/root/.cache/msbuild \
    dotnet restore "RazorApp/Dove.Blog.WebApp.csproj"

# Copy the entire source code and build the application
COPY . .
WORKDIR "/src/RazorApp"
RUN dotnet build "Dove.Blog.WebApp.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Dove.Blog.WebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image for running the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dove.Blog.WebApp.dll"]