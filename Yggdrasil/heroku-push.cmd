call docker build -t yggdrasil-dev .
call heroku container:push -a yggdrasil-dev web
call heroku container:release -a yggdrasil-dev web
call pause