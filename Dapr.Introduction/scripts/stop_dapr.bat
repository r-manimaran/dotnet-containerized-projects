@echo off
echo Stopping Dapr containers...
docker stop dapr_placement
docker stop dapr_redis
docker stop dapr_zipkin
dockeer stop dapr_scheduler
FOR /f "tokens=*" %%i IN ('docker ps --filter name^=daprd --format "{{.Names}}"') DO docker stop %%i
echo Dapr containers stopped
