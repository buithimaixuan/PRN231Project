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
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

# Ch?y ?ng d?ng Web API
ENTRYPOINT ["dotnet", "PostService.dll"]