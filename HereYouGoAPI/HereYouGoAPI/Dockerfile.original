FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY HereYouGoAPI/*.csproj ./HereYouGoAPI/
RUN dotnet restore

# copy everything else and build app
COPY HereYouGoAPI/. ./HereYouGoAPI/
WORKDIR /app/HereYouGoAPI
RUN dotnet publish -c Release -o out


FROM microsoft/dotnet:2.1-HereYouGoAPI-runtime AS runtime
WORKDIR /app
COPY --from=build /app/HereYouGoAPI/out ./
ENTRYPOINT ["dotnet", "HereYouGoAPI.dll"]