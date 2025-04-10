# 📘 CurrencyConverterAPI

A secure, scalable, and maintainable Currency Conversion API built with **ASP.NET Core**, integrating with the [Frankfurter API](https://www.frankfurter.app/docs/). This service allows users to retrieve latest exchange rates, convert between currencies (excluding restricted ones), and fetch historical exchange rates with pagination.

---

## 🚀 Setup Instructions

### 🛠 Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/) *(optional, for containerization)*
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) or VS Code
- [Postman](https://www.postman.com/) or Swagger for testing

---

### 🧪 Local Development (Without Docker)

1. **Clone the repository**
   ```bash
   git clone https://github.com/yurik-sse/CurrencyConverterAPI.git
   cd CurrencyConverterAPI
   ```

2. **Set JWT Secret in `appsettings.json`**
   ```json
   "Jwt": {
     "Key": "ThisIsASecureJwtKeyofYurikAsatryan!"
   }
   ```

3. **Run the API**
   ```bash
   dotnet run
   ```

4. **Visit Swagger UI**
   ```
   http://localhost:5291/swagger
   ```

5. **Generate a test JWT token**
   - The console logs a valid token on startup. Use it as `Bearer <token>` in Swagger or Postman.

---

### 🐳 Docker (Optional)

1. **Build the Docker image**
   ```bash
   docker build -t currency-converter-api .
   ```

2. **Run the container**
   ```bash
   docker run -p 8080:80 currency-converter-api
   ```

3. Access Swagger:
   ```
   http://localhost:8080/swagger
   ```

---

### 🔁 CI/CD (GitHub Actions)

- On each push to `main`, GitHub Actions:
  - Builds and tests the API
  - Builds a Docker image
  - Pushes it to Docker Hub

---

### 🔐 GitHub Secrets Required

Create two secrets in your GitHub repository (`Settings → Secrets → Actions`):

| Name                  | Description                              |
|-----------------------|------------------------------------------|
| `DOCKER_HUB_USERNAME` | Your Docker Hub **username**             |
| `DOCKER_HUB_TOKEN`    | Your Docker Hub **access token**         |

#### 🔑 How to Create a Docker Hub Token:

1. Go to: [Docker Hub Security Settings](https://hub.docker.com/settings/security)
2. Click **"New Access Token"**
3. Choose a name like `github-actions`, set permissions (Read/Write), and generate
4. Copy the token and store it in `DOCKER_HUB_TOKEN`

> ⚠️ **Note:** Using your Docker username in a public repo is safe. Only your token must remain secret.

---

## 🤔 Assumptions Made

- **Currencies excluded**: `TRY`, `PLN`, `THB`, and `MXN` cannot be converted.
- **Frankfurter API** is the only provider (extensible via factory pattern).
- In-memory caching is sufficient for performance at current scale.
- JWT token is pre-generated for demo; no user login/registration.
- The app targets only **server-side use** (client use would require additional rate limiting/auth layers).

---

## 🚧 Possible Future Enhancements

✅ High-priority:
- [ ] Add user registration/login and token issuance endpoint  
- [ ] Role-based endpoints (`Admin`, `User`)  
- [ ] Add **rate limiting** (e.g., using `AspNetCoreRateLimit`)  
- [ ] Support **multiple providers** (via factory pattern)  

✨ Nice-to-have:
- [ ] Docker Compose with Redis for distributed caching  
- [ ] API versioning  
- [ ] Response caching headers  
- [ ] Health check endpoint (`/health`)  
- [ ] Metrics dashboard with Prometheus/Grafana  
- [ ] Deploy to **Render**, **Railway**, or **Azure App Service**

---

## 📎 Reference

- [Frankfurter API](https://www.frankfurter.app/docs/)
- [Serilog](https://serilog.net/)
- [Polly](https://github.com/App-vNext/Polly)
- [OpenTelemetry](https://opentelemetry.io/)
