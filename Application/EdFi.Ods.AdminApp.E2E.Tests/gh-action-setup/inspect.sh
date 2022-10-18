#! /bin/bash
end=$((SECONDS+ 5 * 60))

until [[ "`docker inspect -f {{.State.Health.Status}} ed-fi-ods-adminapp`" == "healthy" || $SECONDS -gt $end ]]; do
    sleep 2;
done;

if [ "`docker inspect -f {{.State.Health.Status}} ed-fi-ods-adminapp`" == "healthy" ]
then
    echo "--- Container is healthy ---"
else
    docker ps
    docker logs ed-fi-ods-adminapp --tail 50
    echo "--- Operation timed out. Review container status ---"
    exit 1
fi
