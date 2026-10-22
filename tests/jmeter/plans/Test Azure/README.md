# Planes de Prueba Apache JMeter para Microsoft Azure

Esta carpeta contiene la suite de pruebas no funcionales de carga y concurrencia calibradas para la infraestructura de **Azure for Students** (App Service F1/B1 y Azure SQL Basic 5 DTUs) del **Sistema Web de Gestión y Visibilidad de Tutorías de UPIICSA**.

---

## Inventario de Planes de Prueba (.jmx)

| Archivo | Escenario Evaluado | Concurrencia Calibrada | Endpoint Objetivo | Métrica Esperada |
| :--- | :--- | :---: | :--- | :--- |
| `JMX-01_Escaneo_QR_Azure.jmx` | Escaneo simultáneo de QR en aula | 15 alumnos | `GET /Asistencia/ValidarQr?token={token}` | $p_{95} < 800$ ms, éxito 100% |
| `JMX-02_Catalogo_Tutorias_Azure.jmx` | Consulta y filtros de tutorías | 25 alumnos | `GET /Catalogo/Index` | Throughput $> 15$ req/s, $p_{95} < 700$ ms |
| `JMX-03_Autenticacion_Login_Azure.jmx` | Inicio de sesión masivo | 12 usuarios | `POST /Account/Login` | $p_{95} < 1200$ ms, 0% errores 500 |
| `JMX-04_Descarga_Reporte_PDF_Azure.jmx` | Generación y descarga de PDF | 3 coordinadores | `GET /Reportes/ExportarMetricasPdf` | Entrega $< 3.5$ s, RAM $< 70\%$ |

---

## Guia de Ejecucion por Consola (CLI)

Para ejecutar cualquiera de los planes apuntando a tu instancia en Azure:

### 1. Ejecucion del Catalogo (JMX-02)
```bash
./tests/jmeter/apache-jmeter-5.6.3/bin/jmeter -n \
  -t "tests/jmeter/plans/Test Azure/JMX-02_Catalogo_Tutorias_Azure.jmx" \
  -Jhost=tu-app-tutorias.azurewebsites.net \
  -Jport=443 \
  -Jprotocol=https \
  -Jthreads=25 \
  -Jrampup=30 \
  -l tests/jmeter/results/azure_catalogo.jtl \
  -e -o tests/jmeter/results/dashboard_azure_catalogo/
```

### 2. Ejecucion de Autenticacion (JMX-03)
```bash
./tests/jmeter/apache-jmeter-5.6.3/bin/jmeter -n \
  -t "tests/jmeter/plans/Test Azure/JMX-03_Autenticacion_Login_Azure.jmx" \
  -Jhost=tu-app-tutorias.azurewebsites.net \
  -Jport=443 \
  -Jprotocol=https \
  -Jthreads=12 \
  -Jrampup=12 \
  -l tests/jmeter/results/azure_login.jtl \
  -e -o tests/jmeter/results/dashboard_azure_login/
```

### 3. Ejecucion de Descarga de Reportes PDF (JMX-04)
```bash
./tests/jmeter/apache-jmeter-5.6.3/bin/jmeter -n \
  -t "tests/jmeter/plans/Test Azure/JMX-04_Descarga_Reporte_PDF_Azure.jmx" \
  -Jhost=tu-app-tutorias.azurewebsites.net \
  -Jport=443 \
  -Jprotocol=https \
  -Jthreads=3 \
  -l tests/jmeter/results/azure_pdf.jtl \
  -e -o tests/jmeter/results/dashboard_azure_pdf/
```

### 4. Ejecucion de Escaneo QR (JMX-01)
```bash
./tests/jmeter/apache-jmeter-5.6.3/bin/jmeter -n \
  -t "tests/jmeter/plans/Test Azure/JMX-01_Escaneo_QR_Azure.jmx" \
  -Jhost=tu-app-tutorias.azurewebsites.net \
  -Jport=443 \
  -Jprotocol=https \
  -Jthreads=15 \
  -Jrampup=15 \
  -Jqr_token=TOKEN_GENERADO_POR_TUTOR \
  -l tests/jmeter/results/azure_qr.jtl \
  -e -o tests/jmeter/results/dashboard_azure_qr/
```

---

## Recomendaciones para la Nube
1. Sustituir `tu-app-tutorias.azurewebsites.net` por el nombre real de tu App Service en Azure.
2. Mantener abierto el portal de Azure durante las ejecuciones para monitorizar el uso de CPU y memoria en App Service y el consumo de DTUs en Azure SQL Database.
