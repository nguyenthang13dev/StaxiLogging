# Staxi.Log

`Staxi.Log` là th? vi?n logging s? d?ng **Serilog** k?t h?p **Elasticsearch** ?? qu?n lý log cho các ?ng d?ng .NET Core / .NET 6+ / .NET Frame. Th? vi?n h? tr?:

- Ghi log vào Elasticsearch.
- H? tr? rolling index d?a trên **alias + ILM policy**.
- T? ??ng fallback log khi Elasticsearch offline.
- Tùy ch?n m?c log (`Information`, `Warning`, `Error`, ...).

---
### I. Cài môi tr??ng
- T?o docker-compose.yml n?u ch?a có elasticsearch,kibana
```bash

services:
  # 1. Elasticsearch
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.11.1
    container_name: elasticsearch
    environment:
      - node.name=elasticsearch
      - cluster.name=es-docker-cluster
      - discovery.type=single-node
      - ELASTIC_PASSWORD=staxicommon123!@#
      - "ES_JAVA_OPTS=-Xms512m -Xmx512m"
      - xpack.security.enabled=false
    volumes:
      - elasticsearch_data:/usr/share/elasticsearch/data
    ports:
      - 9200:9200
      - 9300:9300
    restart: unless-stopped
   

  # 2. Kibana
  kibana:
    image: docker.elastic.co/kibana/kibana:8.11.1
    container_name: kibana
    ports:
      - 5601:5601
    environment:
      -  ELASTICSEARCH_HOSTS=http://elasticsearch:9200
      - ELASTICSEARCH_USERNAME=kibana_system  
      - ELASTICSEARCH_PASSWORD=staxicommon123!@#
      - XPACK_SECURITY_ENABLED=true
    depends_on:
      - elasticsearch
    restart: unless-stopped

  
volumes:
  elasticsearch_data:
    driver: local


```
Run docker-compose
```bash
docker-compose up -d
```

### 1. Cài ??t d? án

Thêm package NuGet:
```bash
dotnet add package Staxi.Log
```
### 2. C?u hình elastic
```bash
        using StaxiLogging.src;
 
         var options = new ElasticLoggingOption
         {
             Uri = "http://localhost:9200",  // Uri
             IndexFormat = "winforms-app-{0:yyyy.MM.dd}", // Index format config theo ngày ngày
             ApplicationName = "WinFormsApp1", // Tên ?ng d?ng
             EnvironmentName = "Dev", // Môi tr??ng
             AutoRegisterTemplate = true, // T? ??ng ??ng ký index template
             NumberOfReplicas = 1, // S? l??ng replicas cho index
             NumberOfShards = 1, 
             MiniLogLevel = LogEventLevel.Debug, // Log t? level
             BatchPostingLimit = 50, 
             PathFileSinkFail = "Logs/serilog-elastic-failures.txt", // ES ch?t s? ??y log vào file
             User = "elastic",       // n?u c?n auth
             Password = "staxicommon123!@#"    // n?u c?n auth
         };
```
### 2. C?u hình Serilog trong ?ng d?ng
- V?i serilog
```bash
  Log.Logger = new LoggerConfiguration(options)
                  .UseElasticLoggingConfig)
                  .CreateLogger();

  builder.Host.UseSerilog();
```

- log4net (d? án ?ang dùng s? redirect lu?ng ghi và serilog)
```bash
            var loggerConfig = new LoggerConfiguration()
                                    .MinimumLevel.Debug()
                                    .UseElasticLoggingConfig(options);
                                    .CreateLogger();
     
            Log.Logger.RedirectLog4Net();

```
- Nlog
```bash
            var loggerConfig = new LoggerConfiguration()
                                                .MinimumLevel.Debug()
                                                .UseElasticLoggingConfig(options);
                                                .CreateLogger();
            Log.Logger.RedirectNLog();
```
### 3, Ki?m tra , truy v?n log
- V?i dev env xem log trên localhost:9200 (ES)
- Xem data, query , t?o báo cáo trên kibana  localhost:5601 (Kibana)
