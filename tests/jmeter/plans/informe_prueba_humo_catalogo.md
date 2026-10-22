# Informe Técnico: Prueba de Humo de Rendimiento del Catálogo de Tutorías

**Proyecto:** Sistema Web de Gestión y Visibilidad de Tutorías de UPIICSA  
**Módulo Evaluado:** Catálogo Público de Tutorías (Requerimiento RF-13)  
**Tipo de Prueba:** Prueba de Humo No Funcional (Smoke Test de Carga Inicial)  
**Herramienta Utilizada:** Apache JMeter 5.6.3 (Modo CLI / Non-GUI)  
**Fecha de Ejecución:** 21 de octubre de 2026  
**Entorno de Ejecución:** Servidor local de desarrollo (.NET 8 Kestrel + SQL Server)

---

## 1. Objetivo de la Prueba

El propósito fundamental de esta prueba de humo fue validar la viabilidad técnica y arquitectónica del módulo del Catálogo de Tutorías previo a la ejecución de pruebas de carga a escala en infraestructura de nube (Microsoft Azure).

Específicamente, la prueba tuvo como metas:
1. Comprobar que la desprotección del catálogo (`[AllowAnonymous]`) funcione correctamente, permitiendo el acceso público sin forzar redirecciones indebidas (`HTTP 302`) hacia la pantalla de inicio de sesión (`/Account/Login`).
2. Validar que los endpoints resuelvan de forma íntegra las consultas a la base de datos relacional, tanto para la carga inicial completa como para las peticiones con filtros dinámicos (búsqueda de texto y filtro por carrera).
3. Obtener una línea base de latencia de red, tiempos de respuesta y estabilidad de la memoria en peticiones concurrentes iniciales.

---

## 2. Parámetros de la Configuración de Prueba

La prueba se diseñó en Apache JMeter bajo el plan `JMX-02_SmokeTest_Catalogo.jmx`, configurado con los siguientes parámetros de ejecución:

| Parámetro | Valor Configurado | Justificación Técnica |
| :--- | :--- | :--- |
| **Servidor Destino** | `localhost` | Aislamiento en entorno de desarrollo controlado. |
| **Puerto de Servicio** | `5178` | Puerto de escucha del servidor Kestrel. |
| **Protocolo** | HTTP | Comunicación de prueba sin sobrecarga de cifrado local. |
| **Usuarios Concurrentes (Hilos)** | 2 hilos | Carga mínima controlada para validación de concurrencia básica. |
| **Periodo de Subida (Ramp-up)** | 2 segundos | Entrada escalonada de los usuarios para evitar colisión de sockets. |
| **Iteraciones por Usuario (Loops)** | 2 ciclos | Verificación de comportamiento repetitivo y calentamiento de caché. |
| **Tiempo de Espera (Think Time)** | Gaussiano (~1000 ms ± 500 ms) | Simulación de la pausa natural de lectura del estudiante. |
| **Total de Peticiones Ejecutadas** | 12 transacciones | 2 usuarios x 2 ciclos x 3 endpoints por ciclo. |

---

## 3. Resultados Cuantitativos Obtenidos

A partir del registro detallado de eventos capturado en `tests/jmeter/results/smoke_catalogo.jtl`, se extrajeron las siguientes métricas:

| Transacción / Muestra | Peticiones | Éxitos | Errores | Tiempo Mínimo | Tiempo Máximo | Tiempo Promedio |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **01 - GET Catalogo Principal** (`/Catalogo/Index`) | 4 | 4 | 0 (0.0%) | 27 ms | 1,433 ms | 478.0 ms |
| **02 - GET Catalogo Filtro Busqueda** (`?busqueda=tutor`) | 4 | 4 | 0 (0.0%) | 20 ms | 196 ms | 96.0 ms |
| **03 - GET Catalogo Filtro Carrera** (`?carreraId=1`) | 4 | 4 | 0 (0.0%) | 18 ms | 74 ms | 34.0 ms |
| **Total Consolidado** | **12** | **12** | **0 (0.0%)** | **18 ms** | **1,433 ms** | **202.6 ms** |

---

## 4. Interpretación Técnica de los Resultados

### 4.1 Comportamiento de Arranque en Frío (Cold Start)
En la primera petición realizada por el primer hilo (`Grupo de Alumnos 1-1`), el tiempo de respuesta registrado fue de **1,433 ms**.
- **Causa:** Corresponde al comportamiento normal de arranque en frío en tecnologías ASP.NET Core y Entity Framework Core, periodo en el cual se inicializan los singletons, se compilan las expresiones de mapeo objeto-relacional y se abre la conexión inicial al pool de SQL Server.

### 4.2 Rendimiento en Régimen Estable (Warm State)
Una vez superada la inicialización del primer ciclo:
- La carga del catálogo principal descendió drásticamente a un rango de **27 ms a 28 ms**.
- El filtrado por texto libre (`busqueda=tutor`) osciló entre **20 ms y 97 ms**.
- El filtrado relacional por carrera (`carreraId=1`) registró valores óptimos entre **18 ms y 24 ms**.
- **Conclusión:** El motor de Entity Framework Core reutilizó eficientemente los planes de ejecución de SQL Server en memoria y el pipeline de Kestrel respondió de forma inmediata.

### 4.3 Tasa de Éxito e Integridad de Datos
- **Tasa de error:** **0.00%**. Ninguna petición resultó en códigos de error HTTP 4xx o 5xx.
- **Aserciones de contenido:** El 100% de las respuestas validó positivamente la aserción de presencia del encabezado HTML del catálogo, confirmando que la vista Razor se renderizó completa con sus componentes visuales y sin excepciones no controladas.

---

## 5. Conclusiones y Próximos Pasos

1. **Aprobación de la Prueba de Humo:** El módulo de Catálogo de Tutorías ha demostrado estabilidad, integridad funcional y tiempos de respuesta altamente competitivos en entorno local.
2. **Factibilidad para Pruebas en la Nube:** Al no presentar fugas de conexión ni bloqueos en la base de datos, el sistema se encuentra en condiciones técnicas para desplegarse hacia Microsoft Azure y proceder con la prueba de carga a escala (25 usuarios concurrentes según lo estipulado en el plan calibrado).
