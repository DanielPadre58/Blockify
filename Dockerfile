FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Blockify/Blockify.csproj", "Blockify/"]
RUN dotnet restore "Blockify/Blockify.csproj"

COPY . .
WORKDIR "/src/Blockify"

RUN dotnet build "Blockify.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blockify.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 5000
EXPOSE 5001

ENTRYPOINT ["dotnet", "Blockify.dll"]
