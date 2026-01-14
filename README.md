# Staxi.Log

`Staxi.Log` là thư viện logging sử dụng **Serilog** kết hợp **Elasticsearch** để quản lý log cho các ứng dụng .NET Core / .NET 6+ / .NET Frame. Thư viện hỗ trợ:

- Ghi log vào Elasticsearch.
- Hỗ trợ rolling index dựa trên **alias + ILM policy**.
- Tự động fallback log khi Elasticsearch offline.
- Tùy chọn mức log (`Information`, `Warning`, `Error`, ...).

---
### I. Cài môi trường
- Tạo docker-compose.yml nếu chưa có elasticsearch,kibana
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

### 1. Cài đặt dự án

Thêm package NuGet:
```bash
dotnet add package Staxi.Log
```
### 2. Cấu hình elastic
```bash
        using StaxiLogging.src;
 
         var options = new ElasticLoggingOption
         {
             Uri = "http://localhost:9200",  // Uri
             IndexFormat = "winforms-app-{0:yyyy.MM.dd}", // Index format config theo ngày ngày
             ApplicationName = "WinFormsApp1", // Tên ứng dựng
             EnvironmentName = "Dev", // Môi trường
             AutoRegisterTemplate = true, // Tự động đăng ký index template
             NumberOfReplicas = 1, // Số lượng replicas cho index
             NumberOfShards = 1, 
             MiniLogLevel = LogEventLevel.Debug, // Log từ level
             BatchPostingLimit = 50, 
             PathFileSinkFail = "Logs/serilog-elastic-failures.txt", // ES chết sẽ đẩy log vào file
             User = "elastic",       // nếu cần auth
             Password = "staxicommon123!@#"    // nếu cần auth
         };
```
### 2. Cấu hình Serilog trong ứng dụng
- Với serilog
```bash
  Log.Logger = new LoggerConfiguration(options)
                  .UseElasticLoggingConfig)
                  .CreateLogger();

  builder.Host.UseSerilog();
```

- log4net (dự án đang dùng sẽ redirect luồng ghi và serilog)
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
### 3, Kiểm tra , truy vấn log
- Với dev env xem log trên localhost:9200 (ES)
- Xem data, query , tạo báo cáo trên kibana  localhost:5601 (Kibana)

### 4. Tạo ILM (Index log) để xóa index theo ngày tháng
    - Ví dụ:
Rollover: 1 ngày HOẶC 5GB
Delete: sau 7 ngày

1. Tạo ILM Policy với index pattern:
```bash
PUT _ilm/policy/applogs-minute-policy
{
  "policy": {
    "phases": {
      "hot": {
        "min_age": "0ms",
        "actions": {
          "rollover": {
            "max_age": "1m"
          }
        }
      },
      "delete": {
        "min_age": "2m",  // Xóa sau 2 phút
        "actions": {
          "delete": {}
        }
      }
    }
  }
}
```
2. Tạo Component Template để tự động gắn ILM:
```bash
PUT _component_template/applogs-ilm-settings
{
  "template": {
    "settings": {
      "index.lifecycle.name": "applogs-minute-policy"
    }
  }
}

PUT _index_template/applogs-minute-template
{
  "index_patterns": ["applogs-*"],
  "composed_of": ["applogs-ilm-settings"],  // Tự động áp dụng ILM
  "priority": 100,
  "template": {
    "settings": {
      "index.number_of_shards": 1,
      "index.number_of_replicas": 1
    }
  }
}
```