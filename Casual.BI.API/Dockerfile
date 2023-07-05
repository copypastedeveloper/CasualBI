FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["casual.BI.API/casual.BI.API.csproj", "casual.BI.API/"]
RUN dotnet restore "casual.BI.API/casual.BI.API.csproj"
COPY . .
WORKDIR "/src/casual.BI.API"
RUN dotnet build "casual.BI.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "casual.BI.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "casual.BI.API.dll"]
