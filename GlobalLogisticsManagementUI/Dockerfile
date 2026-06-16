FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GlobalLogisticsManagementUI/GlobalLogisticsManagementUI.csproj", "GlobalLogisticsManagementUI/"]
RUN dotnet restore "GlobalLogisticsManagementUI/GlobalLogisticsManagementUI.csproj"
COPY . .
WORKDIR "/src/GlobalLogisticsManagementUI"
RUN dotnet build "GlobalLogisticsManagementUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GlobalLogisticsManagementUI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GlobalLogisticsManagementUI.dll"]
