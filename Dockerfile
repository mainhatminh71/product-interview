# --------- BUILD STAGE ---------
    FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
    WORKDIR /src
    
    # Copy solution và project files
    COPY Product.slnx ./
    COPY Product/Product.csproj Product/
    COPY Product.BLL/Product.BLL.csproj Product.BLL/
    COPY Product.DAL/Product.DAL.csproj Product.DAL/
    
    # Restore dependencies
    RUN dotnet restore Product/Product.csproj
    
    # Copy toàn bộ source
    COPY . .
    
    # Build (không publish) để tận dụng cache
    RUN dotnet build Product/Product.csproj -c Release -o /app/build
    
    # Publish
    RUN dotnet publish Product/Product.csproj -c Release -o /app/publish /p:UseAppHost=false
    
    # --------- RUNTIME STAGE ---------
    FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
    WORKDIR /app
    EXPOSE 8080
    
    # Copy file publish từ build stage
    COPY --from=build /app/publish .
    
    # Render thường dùng PORT env, map về 8080
    ENV ASPNETCORE_URLS=http://+:8080
    
    # Entry point
    ENTRYPOINT ["dotnet", "Product.dll"]