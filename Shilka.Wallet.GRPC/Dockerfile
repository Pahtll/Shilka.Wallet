FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Shilka.Wallet.GRPC/Shilka.Wallet.GRPC.csproj", "Shilka.Wallet.GRPC/"]
RUN dotnet restore "Shilka.Wallet.GRPC/Shilka.Wallet.GRPC.csproj"
COPY . .
WORKDIR "/src/Shilka.Wallet.GRPC"
RUN dotnet build "Shilka.Wallet.GRPC.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Shilka.Wallet.GRPC.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shilka.Wallet.GRPC.dll"]
