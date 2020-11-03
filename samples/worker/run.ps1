Push-Location ../../
docker build -t demo-worker:v1 -f ./samples/worker/Dockerfile .
Pop-Location
kubectl apply -f deploy.yaml