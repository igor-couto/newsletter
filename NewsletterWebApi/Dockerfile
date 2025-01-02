# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine3.20-arm64v8 AS build
WORKDIR /source
COPY . .
RUN dotnet --version
RUN ls
RUN dotnet restore NewsletterWebApi --disable-parallel --runtime linux-musl-arm64
RUN dotnet publish NewsletterWebApi -c Release -o /app --no-restore --runtime linux-musl-arm64 --self-contained true

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine3.20-arm64v8 AS run
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_URLS="http://+:50040;https://+:50041"
ENV ASPNETCORE_HTTPS_PORT=50041
ENV ASPNETCORE_HTTP_PORT=50040

WORKDIR /app
COPY --from=build /app .
COPY --from=build /NewsletterWebApi/ /app/NewsletterWebApi
USER dotnetuser

EXPOSE 50040 50041

ENTRYPOINT ["./NewsletterWebApi"]