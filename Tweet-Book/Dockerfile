FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Tweet-Book/Tweet-Book.csproj", "Tweet-Book/"]
RUN dotnet restore "Tweet-Book/Tweet-Book.csproj"
COPY . .
WORKDIR "/src/Tweet-Book"
RUN dotnet build "Tweet-Book.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tweet-Book.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tweet-Book.dll"]