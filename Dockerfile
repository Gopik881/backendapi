FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Elixir.Admin.API/Elixir.Admin.API.csproj", "MyApp/"]
WORKDIR "/src/MyApp"
COPY . .
RUN dotnet publish "Elixir.Admin.API/Elixir.Admin.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Elixir.Admin.API.dll"]
