# Geliþtirme için temel .NET çalýþma zamaný
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Derleme için .NET SDK imajý
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# .csproj dosyasýný kopyala ve restore et
COPY ["ProdcutApi.Presentation/ProdcutApi.Presentation.csproj", "ProdcutApi.Presentation/"]
RUN dotnet restore "ProdcutApi.Presentation/ProdcutApi.Presentation.csproj"

# Tüm dosyalarý kopyala
COPY . .

# Projeyi derle
WORKDIR "/src/ProdcutApi.Presentation"
RUN dotnet build "ProdcutApi.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Yayýnlama aþamasý
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProdcutApi.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Son imaj
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProdcutApi.Presentation.dll"]
