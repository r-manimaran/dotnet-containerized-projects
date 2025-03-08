@echo off
echo Starting Dapr containers...
docker start dapr_placement
docker start dapr_redis
docker start dapr_zipkin
docker start dapr_scheduler
echo Dapr containers started
