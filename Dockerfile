# build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY SmartFinance/SmartFinance.csproj SmartFinance/
RUN dotnet restore SmartFinance/SmartFinance.csproj
COPY SmartFinance/ SmartFinance/
RUN dotnet publish SmartFinance/SmartFinance.csproj -c Release -o /app/publish

# final runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SmartFinance.dll"]