FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY Sonara/Sonara.slnx ./Sonara/
COPY Sonara/Sonara.API/Sonara.API.csproj ./Sonara/Sonara.API/
COPY Sonara/Sonara.Application/Sonara.Application.csproj ./Sonara/Sonara.Application/
COPY Sonara/Sonara.Domain/Sonara.Domain.csproj ./Sonara/Sonara.Domain/
COPY Sonara/Sonara.Infrastructure/Sonara.Infrastructure.csproj ./Sonara/Sonara.Infrastructure/

WORKDIR /src/Sonara
RUN dotnet restore

WORKDIR /src
COPY Sonara/ ./Sonara/

WORKDIR /src/Sonara

RUN dotnet restore Sonara.API/Sonara.API.csproj \
    && dotnet publish Sonara.API/Sonara.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Sonara.API.dll"]
