# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
EXPOSE 11113

# Copy project files
COPY ./UserManagementService/UserManagement.API/UserManagement.API.csproj ./UserManagementService/UserManagement.API/
COPY DataContext/Domain/nupkg/ DataContext/Domain/nupkg/

# Restore NuGet package
RUN dotnet nuget add source /app/DataContext/Domain/nupkg
RUN dotnet restore ./UserManagementService/UserManagement.API/UserManagement.API.csproj

# Copy toàn bộ source code
COPY . .
WORKDIR /app/UserManagementService/UserManagement.API

# Build project
RUN dotnet build -c Release -o /out

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
COPY --from=build /app/UserManagementService/UserManagement.API/Templates /app/Templates

ENTRYPOINT ["dotnet", "UserManagement.API.dll"]
