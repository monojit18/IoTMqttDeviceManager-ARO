FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
# EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY [".", "IotMqttDeviceManager/"]
RUN dotnet restore "IotMqttDeviceManager/IotMqttDeviceManager.csproj"
WORKDIR /src/IotMqttDeviceManager
RUN dotnet build "IotMqttDeviceManager.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "IotMqttDeviceManager.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IotMqttDeviceManager.dll"]