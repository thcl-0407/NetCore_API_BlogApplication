#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["API_BlogApplication/API_BlogApplication.csproj", "API_BlogApplication/"]
COPY ["DAL/DAL.csproj", "DAL/"]
COPY ["Utilities/Utilities.csproj", "Utilities/"]
COPY ["DTO/DTO.csproj", "DTO/"]
COPY ["Services/Services.csproj", "Services/"]
RUN dotnet restore "API_BlogApplication/API_BlogApplication.csproj"
COPY . .
WORKDIR "/src/API_BlogApplication"
RUN dotnet build "API_BlogApplication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API_BlogApplication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API_BlogApplication.dll"]