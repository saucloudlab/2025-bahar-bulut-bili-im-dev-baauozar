# المرحلة الأولى: بيئة التشغيل
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# المرحلة الثانية: بناء الكود
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# نسخ ملفات المشاريع
COPY ["CoreApiService.Api/CoreApiService.Api.csproj", "CoreApiService.Api/"]
COPY ["CoreApiService.Application/CoreApiService.Application.csproj", "CoreApiService.Application/"]
COPY ["CoreApiService.Domain/CoreApiService.Domain.csproj", "CoreApiService.Domain/"]
COPY ["CoreApiService.Infrastructure/CoreApiService.Infrastructure.csproj", "CoreApiService.Infrastructure/"]

# استعادة الحزم
RUN dotnet restore "CoreApiService.Api/CoreApiService.Api.csproj"

# نسخ باقي الملفات والبناء
COPY . .
WORKDIR "/src/CoreApiService.Api"
RUN dotnet build "CoreApiService.Api.csproj" -c Release -o /app/build

# المرحلة الثالثة: النشر
FROM build AS publish
RUN dotnet publish "CoreApiService.Api.csproj" -c Release -o /app/publish

# المرحلة النهائية: التشغيل
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoreApiService.Api.dll"]