FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Contacts.API/Contacts.API.csproj", "Contacts.API/"]
RUN dotnet restore "Contacts.API/Contacts.API.csproj"
COPY . .
WORKDIR "/src/Contacts.API"
RUN dotnet build "Contacts.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Contacts.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Contacts.API.dll"]
