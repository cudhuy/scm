# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
EXPOSE 11119

# Copy project files
COPY ./BranchService/Branch.API/Branch.API.csproj ./BranchService/Branch.API/
COPY DataContext/Domain/nupkg/ DataContext/Domain/nupkg/

# Restore NuGet package
RUN dotnet nuget add source /app/DataContext/Domain/nupkg
RUN dotnet restore ./BranchService/Branch.API/Branch.API.csproj

# Copy toàn bộ source code
COPY . .
WORKDIR /app/BranchService/Branch.API

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

ENTRYPOINT ["dotnet", "Branch.API.dll"]
