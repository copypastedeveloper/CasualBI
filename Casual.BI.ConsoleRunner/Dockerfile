FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Casual.BI.ConsoleRunner/Casual.BI.ConsoleRunner.csproj", "Casual.BI.ConsoleRunner/"]
RUN dotnet restore "Casual.BI.ConsoleRunner/Casual.BI.ConsoleRunner.csproj"
COPY . .
WORKDIR "/src/Casual.BI.ConsoleRunner"
RUN dotnet build "Casual.BI.ConsoleRunner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Casual.BI.ConsoleRunner.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Casual.BI.ConsoleRunner.dll"]
