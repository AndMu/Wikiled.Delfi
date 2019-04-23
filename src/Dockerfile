FROM mcr.microsoft.com/dotnet/core/sdk:2.2.4 AS builder
ARG BUILD_VERSION  
WORKDIR /source

COPY . .
RUN dotnet publish Wikiled.Delfi.Service /p:Version=$BUILD_VERSION --output /app/ --configuration Release

# Stage 2
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.4
WORKDIR /app
COPY --from=builder /app .
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["dotnet", "Wikiled.Delfi.Service.dll"]

