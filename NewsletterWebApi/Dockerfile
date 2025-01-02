# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine3.20-arm64v8 AS build
WORKDIR /source
COPY . .
RUN dotnet restore "NewsletterWebApi.csproj" --disable-parallel --runtime linux-arm64
RUN dotnet publish ".NewsletterWebApi.csproj" -c Release -o /app --no-restore --runtime linux-arm64 --self-contained true

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine3.20-arm64v8 AS run
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_URLS="http://+:50040;https://+:50041"
ENV ASPNETCORE_HTTPS_PORT=50041
ENV ASPNETCORE_HTTP_PORT=50040

RUN apk -U upgrade
RUN adduser -D -h /app dotnetuser
RUN chown -R dotnetuser /app

WORKDIR /app
COPY --from=build /app .
COPY --from=build /NewsletterWebApi/ /app/NewsletterWebApi
USER dotnetuser

EXPOSE 50040 50041

ENTRYPOINT ["./NewsletterWebApi"]