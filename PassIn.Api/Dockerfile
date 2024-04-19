FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Add the tools to the PATH
ENV PATH="${PATH}:/root/.dotnet/tools"

# Install EF Core CLI tools
RUN dotnet tool install --global dotnet-ef --version 8.*
RUN dotnet ef --version

COPY ["PassIn.Api/PassIn.Api.csproj", "PassIn.Api/"]
COPY ["PassIn.Application/PassIn.Application.csproj", "PassIn.Application/"]
COPY ["PassIn.Communication/PassIn.Communication.csproj", "PassIn.Communication/"]
COPY ["PassIn.Exceptions/PassIn.Exceptions.csproj", "PassIn.Exceptions/"]
COPY ["PassIn.Infrastructure/PassIn.Infrastructure.csproj", "PassIn.Infrastructure/"]
RUN dotnet restore "PassIn.Api/PassIn.Api.csproj"
COPY . .
RUN dotnet ef migrations bundle -v --force --project PassIn.Infrastructure


WORKDIR "/src/PassIn.Api"
RUN dotnet build "PassIn.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PassIn.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/efbundle .

ENTRYPOINT ["dotnet", "PassIn.Api.dll"]