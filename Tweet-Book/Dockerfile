FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
COPY ["Tweet-Book/Tweet-Book.csproj", "Tweet-Book/"]
RUN dotnet restore "Tweet-Book/Tweet-Book.csproj"
COPY . .
WORKDIR "/app/Tweet-Book"
RUN dotnet build "Tweet-Book.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tweet-Book.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tweet-Book.dll"]