# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
EXPOSE 11116

COPY ./ApiGatewayService/PublicApiGateway/PublicApiGateway.csproj ./ApiGatewayService/PublicApiGateway/
RUN dotnet restore ./ApiGatewayService/PublicApiGateway/PublicApiGateway.csproj

COPY . .
RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Cấu hình múi giờ
RUN apt-get update && apt-get install -y tzdata \
    && ln -sf /usr/share/zoneinfo/Asia/Ho_Chi_Minh /etc/localtime \
    && echo "Asia/Ho_Chi_Minh" > /etc/timezone \
    && rm -rf /var/lib/apt/lists/*
ENV TZ=Asia/Ho_Chi_Minh


COPY --from=build /out .

ENTRYPOINT ["dotnet", "PublicApiGateway.dll"]
