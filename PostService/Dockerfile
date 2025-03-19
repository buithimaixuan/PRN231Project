

# S? d?ng hình ?nh .NET runtime làm môi tr??ng build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Sao chép file .csproj và khôi ph?c các dependencies
COPY *.csproj ./
RUN dotnet restore

# Sao chép toàn b? source code và build ?ng d?ng
COPY . ./
RUN dotnet publish -c Release -o /out

# S? d?ng hình ?nh .NET runtime nh? h?n ?? ch?y ?ng d?ng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

# Ch?y ?ng d?ng Web API
ENTRYPOINT ["dotnet", "PostService.dll"]