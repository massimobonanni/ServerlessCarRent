# ServerlessCarRent.Console

This project is a console application for managing the Serverless Car Rent platform. It provides various commands to interact with the system.

## Description

The `ServerlessCarRent.Console` application allows administrators to manage cars and environments within the Serverless Car Rent platform. It is built using .NET 8 and leverages the `System.CommandLine` library to handle command-line interactions.

## Available Commands

You can then use the available commands as follows:

```
ServerlessCarRent.Console.exe <command> [options]
```

### `cars`
Manages cars within the platform.

#### `search`
Searches for cars based on the specified criteria.

##### Example

```
ServerlessCarRent.Console cars search --uri <Function Url> --key <Function Key> --plate ABC123 --location "New York" --model "Tesla Model S"
```


### `createenv`

Creates pickup locations and cars based on input file (JSON)

### Json Schema

```json
{
  "locations": [
	{
      "identifier": "ROMA-TERMINI",
      "city": "Rome",
      "location": "Rome Termini station"
    },
    {
      "identifier": "MILAN-LIN",
      "city": "Milan",
      "location": "Milan Linate Airport"
    }
  ],
  "cars": [
	{
      "plate": "MF074PF",
      "model": "Audi A3",
      "location": "MILAN-LIN",
      "costPerHour":1.0,
      "currency": "EUR"
    },
    {
      "plate": "VU096DE",
      "model": "Audi A3",
      "location": "ROMA-TERMINI",
      "costPerHour":1.0,
      "currency": "EUR"
    }
  ]
}
```

### Example

```
ServerlessCarRent.Console.exe createenv --uri <Function Url> --key <Function Key> --file "<json file>"
```


