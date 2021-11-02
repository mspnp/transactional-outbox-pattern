FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Contacts.EventsProcessor/Contacts.EventsProcessor.csproj", "Contacts.EventsProcessor/"]
RUN dotnet restore "Contacts.EventsProcessor/Contacts.EventsProcessor.csproj"
COPY . .
WORKDIR "/src/Contacts.EventsProcessor"
RUN dotnet build "Contacts.EventsProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Contacts.EventsProcessor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Contacts.EventsProcessor.dll"]
