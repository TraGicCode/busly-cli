# Use .NET 9 SDK to build
FROM mcr.microsoft.com/dotnet/sdk:10.0.100 AS build

ARG APP_VERSION=0.1.0
WORKDIR /src

COPY Directory.Build.props ./

# Copy project file and restore dependencies (better layer caching)
COPY src/BuslyCLI.Console/BuslyCLI.Console.csproj ./BuslyCLI.Console/
RUN dotnet restore "BuslyCLI.Console/BuslyCLI.Console.csproj" \
    -r linux-musl-x64

# Copy source code and build
COPY src/BuslyCLI.Console/ ./BuslyCLI.Console/

RUN dotnet publish "./BuslyCLI.Console/BuslyCLI.Console.csproj" \
    -c Release \
    -r linux-musl-x64 \
    -o /app/publish \
    --no-restore \
    -p:PublishSingleFile=true \
    -p:PublishTrimmed=true \
    -p:TrimMode=partial \
    -p:EnableCompressionInSingleFile=true \
    -p:SelfContained=true \
    -p:InformationalVersion=$APP_VERSION

# Use minimal runtime for smaller image size
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine AS final
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Change ownership to non-root user
RUN chown -R $APP_UID:$APP_UID /app
USER $APP_UID

ENTRYPOINT ["./BuslyCLI.Console"]
