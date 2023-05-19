# Dengue Tracer API

[![GitHub Stars](https://img.shields.io/github/stars/LarsKhan/dengue-tracer-api.svg?style=flat-square&logo=github)](https://github.com/LarsKhan/dengue-tracer-api/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/LarsKhan/dengue-tracer-api.svg?style=flat-square&logo=github)](https://github.com/LarsKhan/dengue-tracer-api/network/members)
[![GitHub Issues](https://img.shields.io/github/issues/LarsKhan/dengue-tracer-api.svg?style=flat-square&logo=github)](https://github.com/LarsKhan/dengue-tracer-api/issues)
[![GitHub License](https://img.shields.io/github/license/LarsKhan/dengue-tracer-api.svg?style=flat-square&logo=github)](https://github.com/LarsKhan/dengue-tracer-api/blob/main/LICENSE)


![Flutter](https://img.shields.io/badge/Framework-Flutter-blue?style=flat-square&logo=flutter)
![C#](https://img.shields.io/badge/Language-C%23-brightgreen?style=flat-square&logo=c-sharp)
![Microsoft SQL Server](https://img.shields.io/badge/Database-SQL%20Server-lightgrey?style=flat-square&logo=microsoft-sql-server)

Dengue Tracer API is the backend server for the Dengue Tracer Flutter project. It provides the necessary endpoints and functionalities to support the management and tracking of dengue cases. This API is built using C# and utilizes Microsoft SQL Server as the database.

## :sparkles: Features

- **User Management**: The API allows users to register, log in, and manage their accounts. User roles and permissions are implemented to ensure secure access.

- **Case Management**: Users can report new dengue cases, update case status, and view case details. The API handles the storage and retrieval of case information.

- **Sector Integration**: The API integrates with the sector module to associate cases with specific sectors. Sector data is retrieved from the database and used for case mapping and visualization.

- **Real-time Updates**: The API provides real-time updates on dengue cases, ensuring that users have access to the latest information.

- **Automated Status Updates**: The API automatically updates the status of cases based on predefined rules. If a case's end date is null and it has been more than 6 days since the start date, the status is set to false.

## :computer: Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/LarsKhan/dengue-tracer-api.git
   ```

2. Set up the database:

   - Install Microsoft SQL Server and create a new database.
   - Update the connection string in the `appsettings.json` file with your database details.

3. Build and run the API:

   - Open the solution in Visual Studio.
   - Build the solution to restore NuGet packages.
   - Run the application.

4. Access the API:

   - The API will be running at `http://localhost:5000` by default.
   - Use an API testing tool like Postman to interact with the endpoints.

## :rocket: Contributing

Contributions

 are welcome! If you encounter any issues or have suggestions for improvement, please feel free to open an issue or submit a pull request. Let's make Dengue Tracer better together!

## :page_facing_up: License

This project is licensed under the [MIT License](https://github.com/LarsKhan/dengue-tracer-api/blob/main/LICENSE).
