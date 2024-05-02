# Settlement Booking System API

## Overview
The Settlement Booking System API enables users to schedule and manage bookings within predefined operational hours. It ensures that bookings do not conflict (exceed maxmium simultaneous bookings) and comply with available time slots. The API is designed to be flexible, supporting version-controlled updates that allow for backward compatibility.

## Features
- **Create bookings**: Validate incoming booking requests against business rules.

## Getting Started

### Prerequisites
- .NET 5.0 SDK or higher is required for building and running the application.
- A compatible IDE such as Visual Studio or Visual Studio Code for development.

## Architecture

### Clean Architecture
This project is structured based on the principles of Clean Architecture, ensuring that our application is decoupled, maintainable, and scalable. 

Project's Clean Architecture consists of the following layers:

### Configurations
- **BookingOptions.cs**: Contains configurations used throughout the application, such as settings for booking options.

### Domain
This layer includes the core business logic and entities.
- **Exceptions**
  - **BookingUnavailableException.cs**: Custom exception to handle scenarios where a booking cannot be made.
- **Interfaces**
  - **IBookingRepository.cs**: Interface defining operations related to accessing booking data.
- **Models**
  - **Booking.cs**: Represents the booking entity with all necessary booking details.

### Infrastructure
Handles data access and external integrations, implementing interfaces defined in the domain.
- **Repositories**
  - **BookingRepository.cs**: Concrete implementation of the `IBookingRepository` interface, handling data persistence.

### Services
Contains application logic, serving as an intermediary between the presentation layer and infrastructure or domain layers.
- **DTOs**
  - **BookingRequest.cs**: Data transfer object used to encapsulate booking information from clients.
  - **BookingResponse.cs**: Data transfer object for sending booking data back to clients.
- **Implementations**
  - **ApiLogger.cs**: Service for logging throughout the application.
  - **BookingOptionsService.cs**: Service to manage booking options.
  - **BookingService.cs**: Core service handling business logic related to bookings.
- **Interfaces**
  - **IApiLogger.cs**: Interface for the logging service.
  - **IBookingOptionsService.cs**: Interface to manage booking configurations.
  - **IBookingService.cs**: Interface that defines business operations related to bookings.

### WebApi
The entry point for client interactions, consisting of thin controllers that delegate business operations to the services.
- Files and directories in this layer handle routing, request processing, and response formation, ensuring controllers remain lightweight and business-logic-free.

## Getting Started
[Include steps on setting up and running the project locally]

**Clone the repository:**
		
        git clone https://github.com/ratnabhindi/SettlementBookingServiceApi


### Configuration

Modify the `appsettings.json` file in the `src` directory to adjust the booking system settings:
  ```json
    {
    	"BookingOptions": {
    	 "MaxSimultaneousBookings": 4,
    	 "BookingDuration": "01:00:00",
    	 "StartHour": "09:00",
    	 "EndHour": "17:00"
    	}
    }
```
Configuration Details:
- **MaxSimultaneousBookings:**  The maximum number of bookings that can be handled at the same time without confict.
- **BookingDuration:** The fixed duration for each booking. Here, it is set to one hour.
- **StartHour and EndHour:** Define the operating hours of the booking system. Bookings can only be made between 9am and 5pm with the last one being 4pm.


### Running the Application

Execute the following command in your terminal to run the application: dotnet run --project WebAPI/WebAPI.csproj
This command starts the API server, making it accessible on http://localhost:5243
To access the swagger Api documentation: http://localhost:5243/swagger/index.html

## API Endpoints

### Create Booking

- **URL**: `/api/Booking`
- **Method**: `POST`
- **Required Headers**:
  - `Content-Type`: `application/json`
  - `Accept`: `application/json;v=1`

- **Request Body**:

      {
        "Name": "John Doe",
        "BookingTime": "14:00"
      }
    - **Success Response**:
    - **Code**: `200 OK`
    - **Content**:
      ```json
      {
        "bookingId": "generated-booking-id"
      }
      ```

  - **Error Responses**:
  - **Code**: `400 Bad Request`
  - **Content**:
    ```json
    {"error": "Booking details must be provided."}
    ```
  - **Code**: `409 Conflict`
  - **Content**:
    ```json
    {"error": "Time slot not available."}
    ```


## Versioning

This API uses versioning. content-type: application/json; charset=utf-8; v=1.0 


## Testing

To run the unit tests, navigate to the `tests` directory and execute:
dotnet test


## Logging

Logs are generated using Serilog and are stored in the `Logs` directory within the `WebApi` project. These logs provide detailed information for troubleshooting and monitoring the API's operations.


## Source Control

The complete source code and its version history are available on GitHub at:
https://github.com/ratnabhindi/SettlementBookingServiceApi. This repository includes all historical changes/commits.


