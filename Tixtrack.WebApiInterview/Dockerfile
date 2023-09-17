FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tixtrack.WebApiInterview/Tixtrack.WebApiInterview.csproj", "Tixtrack.WebApiInterview/"]
RUN dotnet restore "Tixtrack.WebApiInterview/Tixtrack.WebApiInterview.csproj"
COPY . .
WORKDIR "/src/Tixtrack.WebApiInterview"
RUN dotnet build "Tixtrack.WebApiInterview.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tixtrack.WebApiInterview.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tixtrack.WebApiInterview.dll"]
