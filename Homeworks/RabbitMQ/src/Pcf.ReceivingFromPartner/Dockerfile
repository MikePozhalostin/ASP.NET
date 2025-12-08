FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Pcf.Common/Pcf.Common.csproj Pcf.Common/

COPY Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.WebHost/Pcf.ReceivingFromPartner.WebHost.csproj Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.WebHost/
COPY Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.Core/Pcf.ReceivingFromPartner.Core.csproj Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.Core/
COPY Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.DataAccess/Pcf.ReceivingFromPartner.DataAccess.csproj Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.DataAccess/
COPY Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.Integration/Pcf.ReceivingFromPartner.Integration.csproj Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.Integration/

RUN dotnet restore Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.WebHost/Pcf.ReceivingFromPartner.WebHost.csproj

COPY . .

WORKDIR /src/Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.WebHost
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Pcf.ReceivingFromPartner.WebHost.dll"]
