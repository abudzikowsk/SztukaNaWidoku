FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

RUN apt-get update && \
    apt-get install -y \
    gconf-service \
    libasound2 \
    libatk1.0-0 \
    libc6 \
    libcairo2 \
    libcups2 \
    libdbus-1-3 \
    libexpat1 \
    libfontconfig1 \
    libgcc1 \
    libgconf-2-4 \
    libgdk-pixbuf2.0-0 \
    libglib2.0-0 \
    libgtk-3-0 \
    libnspr4 \
    libpango-1.0-0 \
    libpangocairo-1.0-0 \
    libstdc++6 \
    libx11-6 \
    libx11-xcb1 \
    libxcb1 \
    libxcomposite1 \
    libxcursor1 \
    libxdamage1 \
    libxext6 \
    libxfixes3 \
    libxi6 \
    libxrandr2 \
    libxrender1 \
    libxss1 \
    libxtst6 \
    ca-certificates \
    fonts-liberation \
    libappindicator1 \
    libnss3 \
    lsb-release \
    xdg-utils \
    wget && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 5204

ENV ASPNETCORE_URLS=http://+:5204

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["SztukaNaWidoku/SztukaNaWidoku.csproj", "SztukaNaWidoku/"]
RUN dotnet restore "SztukaNaWidoku/SztukaNaWidoku.csproj"
COPY . .
WORKDIR "/src/SztukaNaWidoku"
RUN dotnet build "SztukaNaWidoku.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN apt-get update && \
    apt-get install -y curl && \
    curl -sL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs
RUN dotnet publish "SztukaNaWidoku.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root
RUN mkdir -p /app/data
RUN chown -R app:app /app/data
RUN chmod -R 755 /app/data
RUN chown -R app:app /app
RUN chmod -R 755 /app
USER app

ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/Database.db"
ENV ConnectionStrings__HangfireConnection="/app/data/Hangfire.db"
ENTRYPOINT ["dotnet", "SztukaNaWidoku.dll"]
