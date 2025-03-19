<<<<<<< HEAD
﻿# Sử dụng hình ảnh .NET runtime làm môi trường build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Sao chép file .csproj và khôi phục các dependencies
COPY *.csproj ./
RUN dotnet restore

# Sao chép toàn bộ source code và build ứng dụng
COPY . ./
RUN dotnet publish -c Release -o /out

# Sử dụng hình ảnh .NET runtime nhỏ hơn để chạy ứng dụng
=======


# S? d?ng h�nh ?nh .NET runtime l�m m�i tr??ng build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Sao ch�p file .csproj v� kh�i ph?c c�c dependencies
COPY *.csproj ./
RUN dotnet restore

# Sao ch�p to�n b? source code v� build ?ng d?ng
COPY . ./
RUN dotnet publish -c Release -o /out

# S? d?ng h�nh ?nh .NET runtime nh? h?n ?? ch?y ?ng d?ng
>>>>>>> main
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

<<<<<<< HEAD
# Chạy ứng dụng Web API
=======
# Ch?y ?ng d?ng Web API
>>>>>>> main
ENTRYPOINT ["dotnet", "PostService.dll"]