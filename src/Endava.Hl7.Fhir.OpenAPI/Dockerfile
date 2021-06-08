#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Endava.Hl7.Fhir.OpenAPI/Endava.Hl7.Fhir.OpenAPI.csproj", "src/Endava.Hl7.Fhir.OpenAPI/"]
COPY ["src/Endava.Hl7.Fhir.Common.Core/Endava.Hl7.Fhir.Common.Core.csproj", "src/Endava.Hl7.Fhir.Common.Core/"]
COPY ["src/Endava.Hl7.Fhir.Common.Contracts/Endava.Hl7.Fhir.Common.Contracts.csproj", "src/Endava.Hl7.Fhir.Common.Contracts/"]
RUN dotnet restore "src/Endava.Hl7.Fhir.OpenAPI/Endava.Hl7.Fhir.OpenAPI.csproj"
COPY . .
WORKDIR "/src/src/Endava.Hl7.Fhir.OpenAPI"
RUN dotnet build "Endava.Hl7.Fhir.OpenAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Endava.Hl7.Fhir.OpenAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Endava.Hl7.Fhir.OpenAPI.dll"]