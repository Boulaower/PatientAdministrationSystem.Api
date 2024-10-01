PatientAdministrationSystem.Api
Patient Administration System - Backend
This is the backend for the Patient Administration System, built with ASP.NET Core. It provides a RESTful API to manage patient data, including CRUD operations and search functionality.
Technologies Used
•	ASP.NET Core: A cross-platform framework for building modern web applications.
•	Entity Framework Core: An ORM (Object-Relational Mapper) for data access.
•	Swagger: For API documentation and testing.
•	In-Memory Database: For local development and testing without persistent storage.
•	xUnit: For unit testing the backend.
Setup and Installation
Prerequisites
•	Ensure you have .NET 6 SDK or later installed. Download it from .NET.
Installation
1.	Clone the Repository:
git clone https://github.com/your-username/PatientAdministrationSystem.Api.git
cd PatientAdministrationSystem.Api
cd PatientAdministrationSystem 
2.	Restore Dependencies:
dotnet restore
3.	Run the Application:
dotnet run
The backend will run at http://localhost:5272.
API Endpoints
•	GET /api/patients: Retrieve all patients.
•	GET /api/patients/search?name={name}: Search patients by name.
•	GET /api/patients/{id}: Retrieve a patient by their unique ID.
•	POST /api/patients: Create a new patient record.
•	PUT /api/patients/{id}: Update an existing patient record.
•	DELETE /api/patients/{id}: Delete a patient record.
Database Configuration
The backend uses an in-memory database for quick setup and testing. Data is seeded on application start. 
Project Structure
/Controllers       # API controllers for handling HTTP requests (e.g., PatientsController).
/Dtos              # Data Transfer Objects for data passing between layers.
/Entities          # Entity models representing database tables.
/Repositories      # Data access layer using repository pattern.
/Services          # Business logic layer.
/Infrastructure    # Database context and configuration files.
/Tests             # Unit tests for the backend using xUnit.


Github Link https://github.com/Boulaower/PatientAdministrationSystem.Api
Testing :
Example UUID to Test with  4e8e935b-d107-4027-83c5-68a04db398a5

Place the above code withing the ID box to Test 
 
 

