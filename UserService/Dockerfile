# Sử dụng hình ảnh .NET runtime làm môi trường build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Sao chép file .csproj và khôi phục các dependencies
COPY *.csproj ./
RUN dotnet restore

# Sao chép toàn bộ source code và build ứng dụng
COPY . ./
RUN dotnet publish -c Release -o /out

# Sử dụng hình ảnh .NET runtime nhỏ hơn để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

# Chạy ứng dụng Web API
ENTRYPOINT ["dotnet", "UserService.dll"]