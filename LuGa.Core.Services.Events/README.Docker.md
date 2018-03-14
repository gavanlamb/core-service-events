# Docker Compose #
You can utilise to docker-compose to help you develop this application.

Following compose templates are available
1. `docker-compose -f docker-compose.yml up` - builds and runs the whole application
1. `docker-compose -f docker-compose.debug.yml up` - it will create database and setup schema for you - you can debug your application locally though.

*Note:* Run `docker-compose build --force-rm` to rebuild the whole solution. Possibly with `-f` to select correct docker file.

# How to build this docker image #

We are using two stage build with separate image for building and separate runtime image. Due to private NuGet repository you need to build docker image with PAT to access Private NuGet.

`docker build -t luga/events:latest --build-arg LUGA_NUGET_PAT="PAT" .`

# How to run this docker image #

`docker run luga/events:latest  -e "mqtt__password"="password" -e "mqtt__username"="events" -e "mqtt__port"=1883 -e "mqtt__host"="mqtt.luga.online" -e "mqtt__clientid"="events" -e "ConnectionStrings__LuGa"="server=mysql;persistsecurityinfo=True;user id=root;password=example;database=luga"`

# How to publish docker image #

1. Login to docker hub `docker login`
1. Build docker image as per above
1. Publish docker image with `docker push luga/events:latest`

# Deploying on external #

This only applies to our external docker setup

`docker run -d --restart always -e mqtt__password="password" -e mqtt__username="events" -e mqtt__port="1883" -e mqtt__host="mqtt.luga.online" -e mqtt__clientid="events" -e ConnectionStrings__LuGa="connectionString" luga/events:latest`