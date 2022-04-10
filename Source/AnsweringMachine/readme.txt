build the image

docker build --tag answering_machine .

run the container

docker run --name answering_machine -d -p 5000:5000 -e DIRECTUS_TOKEN=secret_token -e THRESHOLD=0.8 --network flask answering_machine