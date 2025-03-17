docker build -t basket.service:v1.0 -f Basket.Service\Dockerfile .

docker run -it --rm -p 8000:8080 basket.service:v1.0

[PosMan]
* Variables
BasketMicroserviceBaseAddress / http://localhost:8000 / https://localhost:8000
* GET
{{BasketMicroserviceBaseAddress}}/123
* POST
{{BasketMicroserviceBaseAddress}}/123
{
	"ProductID" : "4321",
	"ProductName" : "Nike"
}

