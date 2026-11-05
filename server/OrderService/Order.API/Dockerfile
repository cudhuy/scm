# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
EXPOSE 11118

COPY ./OrderService/Order.API/Order.API.csproj ./OrderService/Order.API/
RUN dotnet restore ./OrderService/Order.API/Order.API.csproj

COPY . .
WORKDIR /app/OrderService/Order.API
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

ENTRYPOINT ["dotnet", "Order.API.dll"]