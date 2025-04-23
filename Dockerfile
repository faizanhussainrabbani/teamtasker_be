FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Presentation/TeamTasker.API/TeamTasker.API.csproj", "src/Presentation/TeamTasker.API/"]
COPY ["src/Core/TeamTasker.Application/TeamTasker.Application.csproj", "src/Core/TeamTasker.Application/"]
COPY ["src/Core/TeamTasker.Domain/TeamTasker.Domain.csproj", "src/Core/TeamTasker.Domain/"]
COPY ["src/Core/TeamTasker.SharedKernel/TeamTasker.SharedKernel.csproj", "src/Core/TeamTasker.SharedKernel/"]
COPY ["src/Infrastructure/TeamTasker.Infrastructure/TeamTasker.Infrastructure.csproj", "src/Infrastructure/TeamTasker.Infrastructure/"]
RUN dotnet restore "src/Presentation/TeamTasker.API/TeamTasker.API.csproj"
COPY . .
WORKDIR "/src/src/Presentation/TeamTasker.API"
RUN dotnet build "TeamTasker.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeamTasker.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeamTasker.API.dll"]
