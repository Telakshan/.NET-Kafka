docker network create --attachable -d bridge mydockernetwork

 docker-compose up -d 

#Interactive terminal for the kafka container

 docker container exec -it net-kafka-kafka-1 /bin/bash
