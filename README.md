# ServerlessCarRent
This project use Durable Functions (in particular Durable Entities) to implement a simple Car Rent system.

## Architecture
The following image shown the architecture of the solution.

![](Documentation/Images/Architecture.jpg)

The solution is composed by three logical layers:
* **API Layer**: contains the Azure Functions (in particular Client Functions) that implement the REST API endpoints for the solution;
* **Data Layer**: contains two types of Functions:
    * Durable Entities: implement the entities with state of the solution. Car, PickupLocation and CarRentals are the entity of the solution;
    * Orchestrator Functions: implement the workflow called by the API Layer in which you need to interact with Entities in a synchronous way. One of this is the Rent Car scenario;
* **Integration Layer**: contains Orchestrator Functions and Activity Functions used to communicate with external services.

## Functions

### Clients
In this solution the Client Functions are used to expose a set of APIs (REST) and call orchestrators, entities and Durable Functions platform behind the scenes.  
 
### Orchestrators
In this solution, we use the orchestrators in two different ways:
* to "sinchronize" operations between client and entity: a client can only execute a signal operation against an entity. This means that the client continues its own flow while the entity method of the signal operation will be called in asynchronous way. In some scenarios (e.g. rent a car), we need to wait the result of the operation, and to do that, we use an orchestrator.
* to implement integration flows from our solution to the external world.

### Entities
This solution has three different entities:
* CarEntity: it manages a single car. Its status contains only the current rental (if exist).
* PickupLocationEntity: it manages a pickup location with different cars.
* CarRentalsEntity: it stores all the rental history for a car. Its status contains all the rentals completed for a specific car.

### Activities
This solution uses activities to implement atomic operation for the external services integration (e.g. send an event to Event Grdi).

## Sequence Diagrams

### Rent a car

```mermaid
sequenceDiagram
    actor App
    participant RentCarClient
    participant RentOrchestrator
    participant PickupLocationEntity
    participant CarEntity
    participant DurableFunctionsExtension
    App ->> +RentCarClient : [POST]
    RentCarClient -->> RentOrchestrator : StartNewAsync
    RentOrchestrator ->> +PickupLocationEntity : CallEntityAsync -> RentCar
    critical if car belong to the location and can be rented
        PickupLocationEntity --) CarEntity : SignalEntity -> Rent
        Note over PickupLocationEntity,CarEntity: Fire and forget communication
    end
    PickupLocationEntity ->> -RentOrchestrator : Rent car result
    RentCarClient ->> DurableFunctionsExtension : GetStatusAsync(idOrchestrator)
    critical if orchestrator status is completed
        Note over RentCarClient: Retrieve the orchestrator status<br/>from the GetStatusAsync result
    end
    RentCarClient ->> -App : Rent car response
```

### Rent a car

```mermaid
sequenceDiagram
    actor App
    participant ReturnCarClient
    participant ReturnOrchestrator
    participant CarEntity
    participant CarRentalsEntity
    participant PickupLocationEntity
    participant RentalStatusChangeOrchestrator
    participant DurableFunctionsExtension
    App ->> +ReturnCarClient : [PUT]
    ReturnCarClient -->> ReturnOrchestrator : StartNewAsync
    ReturnOrchestrator ->> +CarEntity : CallEntityAsync -> Return
    critical if car can be returned
        CarEntity --) CarRentalsEntity : SignalEntity -> AddRent
        CarEntity --) RentalStatusChangeOrchestrator: StartAsync
        CarEntity --) PickupLocationEntity : SignalEntity -> CarStatusChanged
        Note over PickupLocationEntity,CarEntity: Fire and forget communications
    end
    CarEntity ->> -ReturnOrchestrator : Return car result
    ReturnCarClient ->> DurableFunctionsExtension : GetStatusAsync(idOrchestrator)
    critical if orchestrator status is completed
        Note over ReturnCarClient: Retrieve the orchestrator status<br/>from the GetStatusAsync result
    end
    ReturnCarClient ->> -App : Response


   
```