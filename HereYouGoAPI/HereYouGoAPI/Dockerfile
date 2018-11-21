FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 56774
EXPOSE 44343

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["HereYouGoAPI/HereYouGoAPI.csproj", "HereYouGoAPI/"]
RUN dotnet restore "HereYouGoAPI/HereYouGoAPI.csproj"
COPY . .
WORKDIR "/src/HereYouGoAPI"
RUN dotnet build "HereYouGoAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "HereYouGoAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "HereYouGoAPI.dll"]