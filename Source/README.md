# Running the Dove Blog Project with Docker

## Prerequisites

- Ensure Docker and Docker Compose are installed on your system.
- Verify that your Docker version supports multi-stage builds.

## Build and Run Instructions

1. Clone the repository and navigate to the project root directory.
2. Build and run the project using Docker Compose:
   ```bash
   docker-compose up --build
   ```
3. Access the application at `http://localhost:8080`.

## Configuration

- The application exposes the following ports:
  - `8080`: Main application endpoint.
  - `8081`: Secondary service endpoint.
- Modify the `docker-compose.yml` file to adjust port mappings or add environment variables as needed.

## Notes

- The project uses .NET version 8.0 as specified in the Dockerfile.
- Ensure any required environment variables are set in a `.env` file or directly in the `docker-compose.yml` file.

For further details, refer to the project documentation or contact the development team.