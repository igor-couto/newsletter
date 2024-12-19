# Newsletter Application

This repository contains a complete newsletter management system. Below is an overview of the project and its functionality.
You can run it locally or visit this publised instance here:
-  [igorcouto.com/newsletter](igorcouto.com/newsletter)
-  [igorcouto.com/newsletter/api/swagger](igorcouto.com/newsletter/api/swagger)

## Projects

### Database

A docker-compose.yml file is provided to quickly set up the database.

### Migrations Project

Built using .NET 9 and the Fluent Migrator package.

Run migrations with the following commands:

```
dotnet run up   # Applies migrations
dotnet run down # Rolls back migrations
```

### RESTful Web API

Built with .NET 9, featuring newsletter endpoints and much more:

- API Key Authorization for protected endpoints.
- Caching for optimized performance.
- Rate Limiting to control API usage.
- OpenAPI with SwaggerUI for API documentation and testing.
- Health Checks to monitor service status.
- Versioning for API lifecycle management.
- Input Validation and Sanitization to ensure data integrity and security.
- CORS Support to handle cross-origin requests.

### Worker Service

A .NET 9 worker using Hangfire for scheduled job processing. It uses a cron expression schedules jobs every 30 minutes to send publications to subscriber emails.

### Shared SDK

A .NET 9 library project that encapsulates business domain code and shared functionalities.

**Note:** This is not a client SDK but a shared library for internal use across projects.

### Frontend

A simple frontend built with HTML, CSS, and JavaScript.

Provides functionalities to create subscribers, publications and management functions.

**Note:** This is a demonstration frontend and not intended for production use.

## How It Works

Publications

Publications are created using HTML. They can be sent to subscribers immediately or scheduled to send at intervals of 30 minutes.

System Flow

The frontend communicates with the RESTful API to manage subscribers and publications.

The Hangfire worker processes scheduled jobs and sends publications via email.

## Getting Started

### Prerequisites

Make sure you have Docker, Docker Compose and .NET 9 SDK

### Running the Newsletter

1.Clone this repository:

`git clone https://github.com/igor-couto/newsletter.git`

2.Navigate to newsletter folder and start the database:

`docker-compose up -d`

3.Navigate to NewsletterMigrations folder and run database migrations:

`dotnet run up`

4.Navigate to NewsletterWebAPI and start the Web API:

`dotnet run`
Now you can access the Swagger UI at http://localhost:5000/api/swagger.
The default API Key is _4D2iz0YC0VBcioHhENgafoyflvUUdTXrDJBmaf3VicPHDNHnbSCZznNo3hQUjQ1W_. I strongly recomend you change it in `appsettings.json` or in your environment variable `AUTHORIZATION__APIKEY` before publishing this API.

5.Open another terminal instance and navigate to NewsletterWorker to start the worker service:

`dotnet run`

6.Open the frontend:

Open the file `NewsletterFrontend/index.html` in a browser.

## Contributing

Feel free to open issues or submit pull requests to improve the project. Contributions are welcome!

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Author

* **Igor Couto** - [igor.fcouto@gmail.com](mailto:igor.fcouto@gmail.com)
