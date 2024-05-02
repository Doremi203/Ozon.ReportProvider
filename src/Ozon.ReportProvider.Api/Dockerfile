FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/Ozon.ReportProvider.Api/Ozon.ReportProvider.Api.csproj", "src/Ozon.ReportProvider.Api/"]
COPY ["src/Ozon.ReportProvider.Bll/Ozon.ReportProvider.Bll.csproj", "src/Ozon.ReportProvider.Bll/"]
COPY ["src/Ozon.ReportProvider.Domain/Ozon.ReportProvider.Domain.csproj", "src/Ozon.ReportProvider.Domain/"]
COPY ["src/Ozon.ReportProvider.Utils/Ozon.ReportProvider.Utils.csproj", "src/Ozon.ReportProvider.Utils/"]
COPY ["src/Ozon.ReportProvider.Dal/Ozon.ReportProvider.Dal.csproj", "src/Ozon.ReportProvider.Dal/"]
COPY ["src/Ozon.ReportProvider.Kafka/Ozon.ReportProvider.Kafka.csproj", "src/Ozon.ReportProvider.Kafka/"]
COPY ["src/Ozon.ReportProvider.Proto/Ozon.ReportProvider.Proto.csproj", "src/Ozon.ReportProvider.Proto/"]
COPY ["Directory.Packages.props", "."]
RUN dotnet restore "src/Ozon.ReportProvider.Api/Ozon.ReportProvider.Api.csproj"
COPY . .
WORKDIR "/src/src/Ozon.ReportProvider.Api"
RUN dotnet build "Ozon.ReportProvider.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ozon.ReportProvider.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ozon.ReportProvider.Api.dll"]
