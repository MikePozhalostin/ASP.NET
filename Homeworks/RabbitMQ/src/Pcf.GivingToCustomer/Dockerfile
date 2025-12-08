FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Pcf.Common/Pcf.Common.csproj Pcf.Common/
COPY Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/Pcf.GivingToCustomer.WebHost.csproj Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/
COPY Pcf.GivingToCustomer/Pcf.GivingToCustomer.Core/Pcf.GivingToCustomer.Core.csproj Pcf.GivingToCustomer/Pcf.GivingToCustomer.Core/
COPY Pcf.GivingToCustomer/Pcf.GivingToCustomer.DataAccess/Pcf.GivingToCustomer.DataAccess.csproj Pcf.GivingToCustomer/Pcf.GivingToCustomer.DataAccess/
COPY Pcf.GivingToCustomer/Pcf.GivingToCustomer.Integration/Pcf.GivingToCustomer.Integration.csproj Pcf.GivingToCustomer/Pcf.GivingToCustomer.Integration/

RUN dotnet restore Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/Pcf.GivingToCustomer.WebHost.csproj

COPY . .

WORKDIR /src/Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Pcf.GivingToCustomer.WebHost.dll"]
