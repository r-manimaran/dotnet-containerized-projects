@echo off
echo Starting Dapr containers...
docker start dapr_placement
docker start dapr_redis
docker start dapr_zipkin
echo Dapr containers started
