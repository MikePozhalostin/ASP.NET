FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Pcf.Common/Pcf.Common.csproj Pcf.Common/
COPY Pcf.Administration/Pcf.Administration.WebHost/Pcf.Administration.WebHost.csproj Pcf.Administration/Pcf.Administration.WebHost/
COPY Pcf.Administration/Pcf.Administration.Core/Pcf.Administration.Core.csproj Pcf.Administration/Pcf.Administration.Core/
COPY Pcf.Administration/Pcf.Administration.DataAccess/Pcf.Administration.DataAccess.csproj Pcf.Administration/Pcf.Administration.DataAccess/

RUN dotnet restore Pcf.Administration/Pcf.Administration.WebHost/Pcf.Administration.WebHost.csproj

COPY . .

WORKDIR /src/Pcf.Administration/Pcf.Administration.WebHost
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Pcf.Administration.WebHost.dll"]
