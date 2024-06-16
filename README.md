

## Description

This is a console-based application designed to manage users and groups within an educational context. It provides functionalities for different user roles including students, teachers, and administrators.

Key features include:

- **User Management**: Users can register, login, and logout. User passwords are securely masked during input for privacy.
- **Role-Based Access Control**: The application supports different user roles, each with its own set of functionalities.
  - **Students** can view their disciplines and check their grades.
  - **Teachers** can view their disciplines, set marks for students, and edit discipline details.
  - **Administrators** have access to additional functionalities including both Student's and Teacher's functionalities, as well as the ability to add new users and groups.
- **Data Storage**: The application uses JSON files for data storage, handled by the `Storage` class. It loads all data at the start of the application and saves it back to the JSON files at appropriate times during the application's lifecycle.

- **Note**: The SuperUser Creds are located at [ApplicationSettings.json](https://git.hits.tsu.ru/AntonyJoseph-Projects/OOPInt-2023_Repass/-/blob/master/Rosario_RepassTask/Storage/ApplicationSettings.json/) file.



## Installation

This project uses .NET 8.0 and requires the following packages:

- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)

To install these packages, use the following commands:

```sh
dotnet add package Newtonsoft.Json 
```

 ## Usage
 ```sh
 dotnet run
 ```


