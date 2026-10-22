# Suite de Pruebas Automatizadas con Selenium WebDriver

Esta carpeta contiene la suite de pruebas funcionales, de interfaz de usuario y de extremo a extremo (E2E) para el **Sistema Web de Gestión y Visibilidad de Tutorías de UPIICSA**, implementada en **.NET 8** utilizando **xUnit** y el patrón **Page Object Model (POM)**.

---

## 1. Estructura del Proyecto

```text
tests/Selenium/
├── tutorias.Tests.Selenium.csproj    <- Archivo de proyecto xUnit
├── appsettings.test.json             <- Configuracion (URLs, navegador, headless)
├── Drivers/
│   └── WebDriverFactory.cs           <- Factoria de navegadores (Firefox / Chrome)
├── PageObjects/                      <- Modelado de vistas y elementos del DOM
│   ├── BasePage.cs
│   ├── LoginPage.cs
│   ├── RegisterPage.cs
│   ├── CatalogoPage.cs
│   ├── CoordinacionPage.cs
│   └── AsistenciaPage.cs
└── Specs/                            <- Casos de prueba automatizados
    ├── SEL01_E2E_CicloTutoriaTests.cs
    ├── SEL02_CatalogoResponsivoTests.cs
    ├── SEL03_ValidacionRegistroTests.cs
    └── SEL04_ControlAccesoRbacTests.cs
```

---

## 2. Configuracion del Entorno (`appsettings.test.json`)

Puedes alternar entre probar en tu entorno local o directamente contra tu aplicación desplegada en **Azure**:

```json
{
  "TestSettings": {
    "BaseUrl": "http://localhost:5000",
    "AzureUrl": "https://tu-app-tutorias.azurewebsites.net",
    "UseAzure": false,
    "Browser": "Firefox",
    "Headless": false,
    "DefaultTimeoutSeconds": 10
  }
}
```

- **`UseAzure`**: Si se cambia a `true`, todas las pruebas correrán automáticamente contra la URL de Azure.
- **`Browser`**: `"Firefox"` o `"Chrome"`.
- **`Headless`**: `false` para ver el navegador abrirse e interactuar visualmente; `true` para ejecución silenciosa en segundo plano.

---

## 3. Instrucciones de Ejecucion

### Opcion A: Ejecutar todas las pruebas con la aplicacion en local
1. En una terminal, levanta tu servidor web local:
   ```bash
   dotnet run
   ```
2. En otra terminal, entra a la carpeta de pruebas y ejecuta:
   ```bash
   dotnet test tests/Selenium/tutorias.Tests.Selenium.csproj
   ```

### Opcion B: Ejecutar una prueba individual
Para ejecutar únicamente el flujo E2E:
```bash
dotnet test tests/Selenium/tutorias.Tests.Selenium.csproj --filter "DisplayName~SEL-01"
```

Para ejecutar la prueba de responsividad móvil:
```bash
dotnet test tests/Selenium/tutorias.Tests.Selenium.csproj --filter "DisplayName~SEL-02"
```

### Opcion C: Ejecutar contra Microsoft Azure
Modifica en `appsettings.test.json` el parámetro `"UseAzure": true` y coloca la URL real de tu App Service, luego ejecuta:
```bash
dotnet test tests/Selenium/tutorias.Tests.Selenium.csproj
```
