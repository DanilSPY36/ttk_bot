#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ttk_bot.csproj", "."]
RUN dotnet restore "./ttk_bot.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./ttk_bot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ttk_bot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
VOLUME ["/app/Photos/DrinkPhotos"]
COPY ./Photos/DrickPhotos /app/Photos/DrinkPhotos
ENTRYPOINT ["dotnet", "ttk_bot.dll"]