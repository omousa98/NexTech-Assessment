# NexTech Coding Assessment

## Description
This project is a full-stack application consisting of an Angular front-end and a C# .NET Core back-end RESTful API. The front-end displays a list of the newest stories styled similarly to Netflix, with features including search and pagination. The back-end provides the stories API with dependency injection, caching, and supports search and pagination. Both front-end and back-end include automated tests to ensure code quality and functionality.

## Table of Contents
- [Description](#description)
- [Table of Contents](#table-of-contents)
- [Usage](#usage)
  - [Running the Back-end API](#running-the-back-end-api)
  - [Running the Front-end App](#running-the-front-end-app)
  - [Running Tests](#running-tests)

## Usage

### Running the Back-end API
1. Navigate to the `Backend` directory:
   ```
   cd Backend
   ```
2. Restore dependencies and run the API:
   ```
   dotnet restore
   dotnet run
   ```
3. The API will be available at `https://localhost:5001` or `http://localhost:5000`.

### Running the Front-end App
1. Navigate to the `Frontend` directory:
   ```
   cd Frontend
   ```
2. Install dependencies:
   ```
   npm install
   ```
3. Run the Angular development server:
   ```
   ng serve
   ```
4. Open your browser and go to `http://localhost:4200` to view the app.

### Running Tests

#### Back-end Tests
1. Navigate to the `Backend.Tests` directory:
   ```
   cd Backend.Tests
   ```
2. Run the tests:
   ```
   dotnet test
   ```

#### Front-end Tests
1. Navigate to the `Frontend` directory:
   ```
   cd Frontend
   ```
2. Run the Angular tests:
   ```
   ng test
