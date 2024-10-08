# Etapa base para rodar a aplica��o ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar os arquivos csproj e restaurar depend�ncias
COPY ../System.Blog.Web/System.Blog.Web.csproj System.Blog.Web/
COPY ../System.Blog.Infrastructure/System.Blog.Infrastructure.csproj System.Blog.Infrastructure/
COPY ../System.Blog.Application/System.Blog.Application.csproj System.Blog.Application/
COPY ../System.Blog.Core/System.Blog.Core.csproj System.Blog.Core/

# Copiar o restante dos arquivos
COPY ../System.Blog.Web/ System.Blog.Web/
COPY ../System.Blog.Infrastructure/ System.Blog.Infrastructure/
COPY ../System.Blog.Application/ System.Blog.Application/
COPY ../System.Blog.Core/ System.Blog.Core/

# Restaurar depend�ncias
RUN dotnet restore System.Blog.Web/System.Blog.Web.csproj

# Build do projeto
WORKDIR /src/System.Blog.Web
RUN dotnet build "./System.Blog.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de publica��o
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./System.Blog.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Atualizar o banco de dados com as migrations
RUN dotnet ef database update -s System.Blog.Web/System.Blog.Web.csproj -p System.Blog.Infrastructure/System.Blog.Infrastructure.csproj

# Etapa final, copiar a publica��o para a imagem base
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Entrypoint da aplica��o
ENTRYPOINT ["dotnet", "System.Blog.Web.dll"]
