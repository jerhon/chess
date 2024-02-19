Invoke-WebRequest http://localhost:5238/swagger/v1/swagger.json -OutFile game-service/swagger.json

kiota generate -l typescript -d game-service/posts-api.yml -c GameClient -o ./game-service/client
