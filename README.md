# shockz.msa

- msa project based on .net 6

## useful commands

- docker commands

```bash
$ cd src
# create and run all containers
$ docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml up -d
# stop and remove all running containers
$ docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml down
# build and run all running containers
$ docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml up --build
# display the list of all running containers
$ docker ps
# display the list of all images
$ docker images
# display the list of all volumes
$ docker volume ls
# stop container
$ docker stop {first 4 char of container id}
# remove container
$ docker rm {first 4 char of container id}
# remove image
$ docker rmi {first 4 char of container id}
# remove volume
$ docker volume rm {volume name}
# stop all containers
$ docker stop $(docker ps -a -q)
# remove all containers
$ docker rm -f $(docker ps -a -q)
# remove all images
$ docker rmi -f $(docker images -a -q)
# remove all volumes
$ docker volume rm $(docker volume ls -q)
# remove all unused containers and images
$ docker system prune
# remove all unused volumnes
$ docker volume prune
```

- package management console

```powershell
# update out dated packages
> Update-Package -ProjectName << Project Name >>
# setup the migration
> Add-Migration InitialCreate -context ConfigurationDbContext
```

- dotnet-ef

```bash
# installing quick start for identityserver4
$ dotnet new -i identityserver4.templates
$ dotnet new is4ui
# installing migration tool
$ dotnet tool install --global dotnet-ef
# create migration for identityserver4
$ dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/PersistedGrantDb
$ dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/ConfigurationDb

# remove migrations for identityserver4
$ dotnet ef migrations remove
```

- mongo & redis

```bash
# mongo bash commands
$ docker exec -it shopping-mongo /bin/bash
$ ls
$ mongo
$ show dbs
$ use CatalogDb
$ db.createCollection('Products')
$ db.Products.insertMany([{ 'Name':'Asus Laptop','Category':'Computers', 'Summary':'Summary', 'Description':'Description', 'ImageFile':'ImageFile', 'Price':54.93 }, { 'Name':'HP Laptop','Category':'Computers', 'Summary':'Summary', 'Description':'Description', 'ImageFile':'ImageFile', 'Price':88.93 } ])
$ db.Products.find({}).pretty()
$ db.Products.remove({})
$ show databases
$ show collections

# redis bash commands
$ docker exec -it aspnetrun-redis /bin/bash
$ redis-cli
$ ping
$ set key value
$ get key
```

- postgresql

```sql
-- database scripts
CREATE TABLE Coupon(
ID SERIAL PRIMARY KEY NOT NULL,
ProductName VARCHAR(24) NOT NULL,
Description TEXT,
Amount INT
);
INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('IPhone X', 'IPhone Discount', 150);
INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('Samsung 10', 'Samsung Discount', 100);
```
