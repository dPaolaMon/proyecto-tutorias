**Casos de prueba de caja negra -- Valores límite -- Tutori-App**

**Cómo usar este documento**

Son 24 casos de prueba, uno por cada requerimiento (RF-01 a RF-24). En
cada uno pruebas el valor que está justo en el borde permitido y los que
están un paso antes o después.

-   **Aceptado:** el sistema te deja continuar sin error.

-   **Rechazado:** el sistema te bloquea y muestra un mensaje de error.

-   **Estado:** cambia \"Pendiente\" a \"Aprobado\" si lo que pasó
    coincide con lo esperado, o a \"Fallido\" si no. En \"Resultado
    obtenido\" anota qué pasó (mensaje que salió, captura de pantalla).

-   **(supuesto):** el PDF no define ese límite. Confírmalo con quien
    desarrolló el sistema y cambia los valores si son distintos.

-   **Menos de 6 filas:** algunos requerimientos solo tienen uno o dos
    límites reales (por ejemplo, \"0, 1 o 2 registros\"). En esos casos
    solo incluí los valores que tiene sentido probar.

-   **Tiempos:** usa el cronómetro del celular. Si no logras ser exacto
    al segundo, repite la prueba con 10 segundos de margen.

**Resumen de los casos**

  ------------------------------------------------------------------------------
  **Caso**   **RF**   **Qué se prueba**                  **Rango válido**
  ---------- -------- ---------------------------------- -----------------------
  CP-01      RF-01    Largo de la contraseña al          8 a 20 caracteres
                      registrarse                        (supuesto)

  CP-02      RF-02    Largo de la contraseña al iniciar  8 a 20 caracteres
                      sesión                             (supuesto)

  CP-03      RF-03    Minutos con la sesión abierta      1 a 30 minutos
                      antes de cerrarla                  (supuesto)

  CP-04      RF-04    Tiempo de vigencia del código de   Hasta 5 minutos
                      recuperación                       

  CP-05      RF-05    Solicitudes pendientes al enviar   0 (solo se permite una
                      una nueva                          a la vez)

  CP-06      RF-06    Solicitudes pendientes por revisar 1 o más

  CP-07      RF-07    Tutores activos                    1 o más

  CP-08      RF-08    Materias al crear el perfil        1 a 5 (supuesto)

  CP-09      RF-09    Materias al actualizar el perfil   1 a 5 (supuesto)

  CP-10      RF-10    Perfiles públicos existentes       1

  CP-11      RF-11    Horario nuevo frente a uno ya      Sin empalme
                      registrado                         

  CP-12      RF-12    Hora de inicio al modificar un     12:00 a 13:59
                      horario                            

  CP-13      RF-13    Horarios registrados               1 o más

  CP-14      RF-14    Resultados de la búsqueda (y       0 o más, en máximo 3
                      tiempo)                            segundos

  CP-15      RF-15    Dígitos del número de boleta       10 (supuesto)

  CP-16      RF-16    Asistencias pendientes de          1 o más
                      confirmar                          

  CP-17      RF-17    Hora al generar el QR frente al    Dentro del horario
                      horario de atención                

  CP-18      RF-18    Tiempo desde que se generó el QR   Hasta 3 minutos

  CP-19      RF-19    Registros en el historial          0 o más

  CP-20      RF-20    Estrellas de calificación          0 a 5

  CP-21      RF-21    Promedio global de calificaciones  0 a 5

  CP-22      RF-22    Largo del mensaje de la            1 a 500 caracteres
                      notificación                       (supuesto)

  CP-23      RF-23    Notificaciones en la bandeja       0 o más

  CP-24      RF-24    Registros para el reporte (y       1 o más, en máximo 10
                      tiempo)                            segundos
  ------------------------------------------------------------------------------

**Módulo 1: Registro, sesión y contraseña (RF-01 a RF-04)**

**Caso de prueba: CP-01 (RF-01 Registrar usuarios)**

-   **Nombre:** Validación de valores límite del largo de la contraseña
    al registrarse.

-   **Objetivo:** Verificar que el sistema acepte contraseñas con un
    largo dentro del rango permitido y rechace las más cortas o más
    largas.

-   **Precondiciones:** Estar en la pantalla de registro. Usar un correo
    institucional nuevo en cada prueba (por ejemplo
    prueba1@alumno.ipn.mx, prueba2@alumno.ipn.mx...) y llenar bien el
    resto de los campos.

-   **Rango válido:** 8 a 20 caracteres (supuesto).

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Escribe la contraseña de la tabla, presiona
    \"Registrarse\" y anota el mensaje que sale.

  -------------------------------------------------------------------------
  **Dato de entrada**        **Resultado       **Resultado     **Estado**
                             esperado**        obtenido**      
  -------------------------- ----------------- --------------- ------------
  Mínimo - 1: 7 caracteres   Rechazado: no se  Acepta          Rechazado
  (Abcde12)                  crea la cuenta y  contraseñas de  
                             sale mensaje de   1 a 7           
                             error             caracteres      

  Mínimo: 8 caracteres       Aceptado: mensaje Acepta          Aceptado
  (Abcdef12)                 de registro       contraseñas de  
                             exitoso           8 caracteres    

  Mínimo + 1: 9 caracteres   Aceptado: mensaje Acepta          Aceptado
  (Abcdefg12)                de registro       contraseñas de  
                             exitoso           9 caracteres    

  Máximo - 1: 19 caracteres  Aceptado: mensaje Acepta          Aceptado
  (Abcdefghijklmno1234)      de registro       contraseñas de  
                             exitoso           19 caracteres   

  Máximo: 20 caracteres      Aceptado: mensaje Acepta          Aceptado
  (Abcdefghijklmno12345)     de registro       contraseñas de  
                             exitoso           20 caracteres   

  Máximo + 1: 21 caracteres  Rechazado: no se  Acepta          Rechazado
  (Abcdefghijklmno123456)    crea la cuenta y  contraseñas de  
                             sale mensaje de   21 caracteres   
                             error                             
  -------------------------------------------------------------------------

**C1:**

![](media/image1.png){width="6.1375in" height="3.452777777777778in"}

![](media/image2.png){width="6.1375in" height="3.452777777777778in"}

**C2:**

![](media/image3.png){width="6.1375in" height="3.452777777777778in"}

![](media/image4.png){width="6.1375in" height="3.452777777777778in"}

**C3:**

![](media/image5.png){width="6.1375in" height="3.452777777777778in"}

![](media/image6.png){width="6.1375in" height="3.452777777777778in"}

**C4:**

![](media/image7.png){width="6.1375in" height="3.452777777777778in"}

![](media/image8.png){width="6.1375in" height="3.452777777777778in"}

**C5:**

![](media/image9.png){width="6.1375in" height="3.452777777777778in"}

![](media/image10.png){width="6.1375in" height="3.452777777777778in"}

**C6:**

![](media/image11.png){width="6.1375in" height="3.452777777777778in"}

![](media/image12.png){width="6.1375in" height="3.452777777777778in"}

**Caso de prueba: CP-02 (RF-02 Iniciar sesión)**

-   **Nombre:** Validación de valores límite del largo de la contraseña
    al iniciar sesión.

-   **Objetivo:** Verificar que el sistema deje entrar con contraseñas
    de largo permitido y no deje entrar cuando la contraseña escrita es
    más corta o más larga que la real.

-   **Precondiciones:** Tener las cuentas creadas en CP-01 con
    contraseñas de 8, 9, 19 y 20 caracteres, sin sesión iniciada.

-   **Rango válido:** 8 a 20 caracteres (supuesto).

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Escribe el correo de la cuenta indicada y la
    contraseña de la tabla, presiona \"Iniciar sesión\". Si entras,
    cierra sesión antes de la siguiente prueba.

  --------------------------------------------------------------------------
  **Dato de entrada**          **Resultado      **Resultado     **Estado**
                               esperado**       obtenido**      
  ---------------------------- ---------------- --------------- ------------
  Mínimo - 1: cuenta de 8      Rechazado:       Se muestra      Rechazado
  caracteres, escribiendo solo mensaje \"Correo mensaje         
  7 (Abcdef1)                  o contraseña     "Contraseña     
                               incorrectos\"    Incorrecta"     

  Mínimo: cuenta de 8          Aceptado: entra  Entra a panel   Aceptado
  caracteres (Abcdef12)        a su panel       principal       
                               principal                        

  Mínimo + 1: cuenta de 9      Aceptado: entra  Entra a panel   Aceptado
  caracteres (Abcdefg12)       a su panel       principal       
                               principal                        

  Máximo - 1: cuenta de 19     Aceptado: entra  Entra a panel   Aceptado
  caracteres                   a su panel       principal       
  (Abcdefghijklmno1234)        principal                        

  Máximo: cuenta de 20         Aceptado: entra  Entra a panel   Aceptado
  caracteres                   a su panel       principal       
  (Abcdefghijklmno12345)       principal                        

  Máximo + 1: cuenta de 20     Rechazado:       Entra a panel   Aceptado
  caracteres, escribiendo 21   mensaje \"Correo principal       
  (Abcdefghijklmno123456)      o contraseña                     
                               incorrectos\"                    
  --------------------------------------------------------------------------

**C1:** ![](media/image13.png){width="6.1375in"
height="3.452777777777778in"}

![](media/image14.png){width="6.1375in" height="3.452777777777778in"}

**C2:**

![](media/image15.png){width="6.1375in" height="3.452777777777778in"}

![](media/image16.png){width="6.1375in" height="3.452777777777778in"}

**C3:**

![](media/image17.png){width="6.1375in" height="3.452777777777778in"}

![](media/image18.png){width="6.1375in" height="3.452777777777778in"}

**C4:**

![](media/image19.png){width="6.1375in" height="3.452777777777778in"}

![](media/image20.png){width="6.1375in" height="3.452777777777778in"}

**C5:**

![](media/image21.png){width="6.1375in" height="3.452777777777778in"}

![](media/image22.png){width="6.1375in" height="3.452777777777778in"}

**C6:**

![](media/image23.png){width="6.1375in" height="3.452777777777778in"}

![](media/image24.png){width="6.1375in" height="3.452777777777778in"}

**Caso de prueba: CP-03 (RF-03 Finalizar sesión)**

-   **Nombre:** Validación de valores límite del tiempo con la sesión
    abierta antes de cerrarla.

-   **Objetivo:** Verificar que el botón \"Cerrar sesión\" funcione
    dentro del tiempo de sesión permitido y que, pasado ese tiempo, el
    sistema ya no conserve la sesión.

-   **Precondiciones:** Haber iniciado sesión con cualquier cuenta. Para
    cada prueba, inicia sesión de nuevo y deja la pantalla abierta sin
    tocar nada el tiempo indicado. Puedes correr las pruebas largas al
    mismo tiempo en navegadores distintos.

-   **Rango válido:** 1 a 30 minutos de sesión abierta (supuesto: el PDF
    no dice cuándo caduca la sesión; confírmalo).

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Espera el tiempo indicado, presiona \"Cerrar
    sesión\" y luego el botón \"Atrás\" del navegador. No debe mostrar
    páginas privadas.

  ---------------------------------------------------------------------------
  **Dato de         **Resultado esperado**     **Resultado       **Estado**
  entrada**                                    obtenido**        
  ----------------- -------------------------- ----------------- ------------
  Mínimo - 1: 0     Aceptado: la sesión se     La sesión se      Aceptado
  minutos (cerrar   cierra (no existe un       cierra            
  sesión apenas     límite inferior real) y    correctamente     
  entras)           \"Atrás\" pide iniciar                       
                    sesión                                       

  Mínimo: 1 minuto  Aceptado: la sesión se     La sesión se      Aceptado
                    cierra y \"Atrás\" pide    cierra            
                    iniciar sesión             correctamente     

  Mínimo + 1: 2     Aceptado: la sesión se     La sesión se      Aceptado
  minutos           cierra y \"Atrás\" pide    cierra            
                    iniciar sesión             correctamente     

  Máximo - 1: 29    Aceptado: la sesión se     La sesión se      Aceptado
  minutos           cierra y \"Atrás\" pide    cierra            
                    iniciar sesión             correctamente     

  Máximo: 30        Aceptado: la sesión se     La sesión se      Aceptado
  minutos           cierra y \"Atrás\" pide    cierra            
                    iniciar sesión             correctamente     

  Máximo + 1: 31    Rechazado: la sesión ya    La sesión se      Aceptado
  minutos           venció y el sistema pide   cierra            
                    iniciar sesión de nuevo    correctamente     
  ---------------------------------------------------------------------------

**C1:**

![](media/image25.png){width="6.1375in" height="3.452777777777778in"}

![](media/image26.png){width="6.1375in" height="3.452777777777778in"}

![](media/image27.png){width="6.1375in" height="3.452777777777778in"}

**C2:**

![](media/image28.png){width="6.1375in" height="3.452777777777778in"}

![](media/image29.png){width="6.1375in" height="3.452777777777778in"}

![](media/image30.png){width="6.1375in" height="3.452777777777778in"}

**C3:**

![](media/image31.png){width="6.1375in" height="3.452777777777778in"}

![](media/image32.png){width="6.1375in" height="3.452777777777778in"}

![](media/image33.png){width="6.1375in" height="3.452777777777778in"}

![](media/image34.png){width="6.1375in" height="3.452777777777778in"}

**C4:**

![](media/image35.png){width="6.1375in" height="3.452777777777778in"}

![](media/image36.png){width="6.1375in" height="3.452777777777778in"}

![](media/image37.png){width="6.1375in" height="3.452777777777778in"}

![](media/image38.png){width="6.1375in" height="3.452777777777778in"}

![](media/image39.png){width="6.1375in" height="3.452777777777778in"}

![](media/image40.png){width="6.1375in" height="3.452777777777778in"}

**C5:**

![](media/image41.png){width="6.1375in" height="3.452777777777778in"}

![](media/image42.png){width="6.1375in" height="3.452777777777778in"}

![](media/image43.png){width="6.1375in" height="3.452777777777778in"}

![](media/image44.png){width="6.1375in" height="3.452777777777778in"}

**C6:**

![](media/image45.png){width="6.1375in" height="3.452777777777778in"}

![](media/image46.png){width="6.1375in" height="3.452777777777778in"}

**Caso de prueba: CP-04 (RF-04 Restablecer contraseña)**

-   **Nombre:** Validación de valores límite del tiempo de vigencia del
    código de recuperación.

-   **Objetivo:** Verificar que el código enviado al correo funcione
    dentro de los 5 minutos permitidos y deje de funcionar después.

-   **Precondiciones:** Tener una cuenta registrada y acceso a su correo
    institucional. Para cada prueba, pide un código nuevo en
    \"¿Olvidaste tu contraseña?\" y empieza a contar desde que presionas
    \"Enviar código\".

-   **Rango válido:** Desde que se envía el código hasta 5 minutos (300
    segundos).

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Cuando el cronómetro marque el tiempo de la tabla,
    escribe el código que llegó al correo, escribe la nueva contraseña y
    confirma.

  -------------------------------------------------------------------------
  **Dato de entrada**    **Resultado esperado**  **Resultado   **Estado**
                                                 obtenido**    
  ---------------------- ----------------------- ------------- ------------
  Mínimo - 1: antes de   No aplica: el código    No aplica     No aplica
  pedir el código (0     todavía no existe                     
  segundos)                                                    

  Mínimo: usar el código Aceptado: permite       Por ejecutar  Pendiente
  apenas llega al correo cambiar la contraseña                 

  Máximo - 1: 4 minutos  Aceptado: permite       Por ejecutar  Pendiente
  59 segundos (299 s)    cambiar la contraseña                 

  Máximo: 5 minutos      Aceptado: permite       Por ejecutar  Pendiente
  exactos (300 s)        cambiar la contraseña                 

  Máximo + 1: 5 minutos  Rechazado: mensaje de   Por ejecutar  Pendiente
  1 segundo (301 s)      código vencido o                      
                         inválido                              
  -------------------------------------------------------------------------

**Módulo 2: Solicitudes y estatus de tutor (RF-05 a RF-07)**

**Caso de prueba: CP-05 (RF-05 Solicitar alta como tutor)**

-   **Nombre:** Validación de valores límite de solicitudes pendientes
    al pedir el alta como tutor.

-   **Objetivo:** Verificar que el sistema acepte la primera solicitud y
    rechace una segunda mientras la primera sigue sin resolverse.

-   **Precondiciones:** Tener sesión iniciada como alumno o docente que
    no sea tutor activo y que no tenga solicitudes pendientes.

-   **Rango válido:** 0 solicitudes pendientes antes de enviar (solo se
    permite una a la vez).

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra al módulo de postulación, envía la solicitud
    y confirma. Luego intenta enviar otra sin que Coordinación haya
    respondido la primera.

  ---------------------------------------------------------------------------
  **Dato de entrada** **Resultado           **Resultado          **Estado**
                      esperado**            obtenido**           
  ------------------- --------------------- -------------------- ------------
  Mínimo: 0           Aceptado: mensaje de  Se muestra mensaje   Aceptado
  solicitudes         solicitud enviada y   de solicitud enviada 
  pendientes (primer  estatus \"Pendiente\" y estatus            
  envío)                                    \"Pendiente\"        

  Mínimo + 1: 1       Rechazado: mensaje de Se muestra mensaje   Aceptado
  solicitud pendiente que ya tienes una     de que ya tienes una 
  (segundo envío      postulación en        postulación en       
  seguido)            proceso               proceso              
  ---------------------------------------------------------------------------

**C1:**

![](media/image47.png){width="6.145833333333333in"
height="3.4583333333333335in"}

![](media/image48.png){width="6.145833333333333in"
height="3.4583333333333335in"}

![](media/image49.png){width="6.145833333333333in"
height="3.4583333333333335in"}

C2:

![](media/image50.png){width="6.145833333333333in"
height="3.4583333333333335in"}

![](media/image51.png){width="6.145833333333333in"
height="3.4583333333333335in"}

**Caso de prueba: CP-06 (RF-06 Gestionar solicitudes de tutoría)**

-   **Nombre:** Validación de valores límite de la cantidad de
    solicitudes pendientes por revisar.

-   **Objetivo:** Verificar que Coordinación pueda autorizar o rechazar
    solicitudes cuando hay una o varias, y que no haya errores cuando no
    hay ninguna.

-   **Precondiciones:** Tener sesión iniciada como Coordinación. Para
    las pruebas de 1 y 2 solicitudes, crea las solicitudes con cuentas
    distintas (CP-05).

-   **Rango válido:** 1 o más solicitudes pendientes.

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra al módulo de solicitudes de tutor y revisa la
    lista antes y después de resolver.

  ---------------------------------------------------------------------------
  **Dato de         **Resultado esperado**  **Resultado          **Estado**
  entrada**                                 obtenido**           
  ----------------- ----------------------- -------------------- ------------
  Mínimo - 1: 0     Lista vacía y sin       Se muestra mensaje   Aceptado
  solicitudes       errores: no hay nada    "No se encontraron   
  pendientes        que autorizar o         solicitudes"         
                    rechazar                                     

  Mínimo: 1         Aceptado: el usuario    El usuario el        Aceptado
  solicitud         queda como \"Tutor      usuario aprobado     
  pendiente, la     Aprobado\", el usuario  recibe el rol de     
  autorizas         aprobado recibe el rol  Tutor                
                    de Tutor                                     

  Mínimo + 1: 2     Aceptado: una queda     Una solicitud es     Aceptado
  solicitudes       \"Tutor Aprobado\" y la aprobada y la otra   
  pendientes,       otra \"Rechazado\"; el  rechazada; el        
  autorizas una y   usuario aprobado recibe usuario aprobado     
  rechazas otra     el rol de Tutor         recibe el rol de     
                                            Tutor                
  ---------------------------------------------------------------------------

**C1:**

![](media/image52.png){width="6.1375in" height="3.452777777777778in"}

**C2:**

![](media/image53.png){width="6.1375in" height="3.452777777777778in"}

![](media/image54.png){width="6.1375in" height="3.452777777777778in"}

![](media/image55.png){width="6.1375in" height="3.452777777777778in"}

**C3:**

![](media/image56.png){width="6.1375in" height="3.452777777777778in"}

![](media/image57.png){width="6.1375in" height="3.452777777777778in"}

![](media/image58.png){width="6.1375in" height="3.452777777777778in"}

![](media/image59.png){width="6.1375in" height="3.452777777777778in"}

![](media/image60.png){width="6.1375in" height="3.452777777777778in"}

**Caso de prueba: CP-07 (RF-07 Revocar estatus de tutor)**

-   **Nombre:** Validación de valores límite de la cantidad de tutores
    activos al revocar el estatus.

-   **Objetivo:** Verificar que Coordinación pueda quitar el estatus
    cuando hay uno o varios tutores activos, y que no haya errores si no
    hay ninguno.

-   **Precondiciones:** Tener sesión iniciada como Coordinación. Tener
    tutores activos según la prueba (usa CP-06 para crearlos).

-   **Rango válido:** 1 o más tutores activos.

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra a la gestión de tutores, busca al tutor,
    elige revocar y confirma en la ventana de advertencia.

  --------------------------------------------------------------------------
  **Dato de          **Resultado esperado**     **Resultado     **Estado**
  entrada**                                     obtenido**      
  ------------------ -------------------------- --------------- ------------
  Mínimo - 1: 0      Sin resultados al buscar   No permite      Rechazado
  tutores activos    un tutor y sin errores: no revocar rol de  
                     hay a quién revocar        tutor           

  Mínimo: 1 tutor    Aceptado: el tutor pierde  No permite      Rechazado
  activo, lo revocas sus privilegios y ya no    revocar rol de  
                     puede usar sus módulos     tutor           

  Mínimo + 1: 2      Aceptado: solo ese tutor   No permite      Rechazado
  tutores activos,   pierde sus privilegios; el revocar rol de  
  revocas solo uno   otro sigue activo          tutor           
  --------------------------------------------------------------------------

**C1:**

**C2:**

**C3:**

**Módulo 3: Perfil público del tutor (RF-08 a RF-10)**

**Caso de prueba: CP-08 (RF-08 Registrar perfil público del tutor)**

-   **Nombre:** Validación de valores límite de la cantidad de materias
    al crear el perfil.

-   **Objetivo:** Verificar que el sistema acepte perfiles con una
    cantidad de materias dentro del rango y rechace los que no traen
    materias o traen de más.

-   **Precondiciones:** Tener sesión iniciada como tutor aprobado que
    todavía no tiene perfil público. Después de cada prueba aceptada,
    elimina el perfil (CP-10) para poder crear otro. Llena bien carrera
    y academia.

-   **Rango válido:** 1 a 5 materias (supuesto).

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra a la configuración del perfil público, agrega
    la cantidad de materias indicada y presiona \"Guardar Perfil\".

  --------------------------------------------------------------------------
  **Dato de   **Resultado esperado**   **Resultado obtenido**   **Estado**
  entrada**                                                     
  ----------- ------------------------ ------------------------ ------------
  Mínimo - 1: Rechazado: mensaje \"Por Permite publicar perfil  Rechazado
  0 materias  favor complete todos los sin tener materias       
              campos requeridos\"      inscritas                

  Mínimo: 1   Aceptado: mensaje \"Tu   Se muestra mensaje \"Tu  Aceptado
  materia     perfil académico y       perfil académico y       
              materias se han guardado materias se han guardado 
              exitosamente.\"          exitosamente.\"          

  Mínimo + 1: Aceptado: mensaje \"Tu   Se muestra mensaje \"Tu  Aceptado
  2 materias  perfil académico y       perfil académico y       
              materias se han guardado materias se han guardado 
              exitosamente.\"          exitosamente.\"          

  Máximo - 1: Aceptado: mensaje \"Tu   Se muestra mensaje \"Tu  Aceptado
  4 materias  perfil académico y       perfil académico y       
              materias se han guardado materias se han guardado 
              exitosamente.\"          exitosamente.\"          

  Máximo: 5   Aceptado: mensaje \"Tu   Se muestra mensaje \"Tu  Aceptado
  materias    perfil académico y       perfil académico y       
              materias se han guardado materias se han guardado 
              exitosamente.\"          exitosamente.\"          

  Máximo + 1: Rechazado: mensaje de    Se muestra mensaje \"Tu  Rechazado
  6 materias  que se superó el límite  perfil académico y       
              de materias              materias se han guardado 
                                       exitosamente.\"          
  --------------------------------------------------------------------------

**C1:**

**C2:**

![](media/image61.png){width="6.1375in" height="3.452777777777778in"}

![](media/image62.png){width="6.1375in" height="3.452777777777778in"}

**C3:**

![](media/image63.png){width="6.1375in" height="3.452777777777778in"}

![](media/image64.png){width="6.1375in" height="3.452777777777778in"}

**C4:**

![](media/image65.png){width="6.1375in" height="3.452777777777778in"}

![](media/image66.png){width="6.1375in" height="3.452777777777778in"}

**C5:**

![](media/image67.png){width="6.1375in" height="3.452777777777778in"}

![](media/image68.png){width="6.1375in" height="3.452777777777778in"}

**C6:**

![](media/image69.png){width="6.1375in" height="3.452777777777778in"}

![](media/image70.png){width="6.1375in" height="3.452777777777778in"}

**Caso de prueba: CP-09 (RF-09 Actualizar perfil público del tutor)**

-   **Nombre:** Validación de valores límite de la cantidad de materias
    al actualizar el perfil.

-   **Objetivo:** Verificar que el sistema acepte actualizar el perfil
    con una cantidad de materias dentro del rango y rechace las que
    quedan fuera.

-   **Precondiciones:** Tener sesión iniciada como tutor aprobado con un
    perfil público ya creado que tenga 1 materia.

-   **Rango válido:** 1 a 5 materias (supuesto).

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra a tu perfil público, deja las materias en la
    cantidad indicada y presiona \"Actualizar Cambios\". Revisa el
    perfil después de cada intento.

  --------------------------------------------------------------------------
  **Dato de      **Resultado esperado** **Resultado obtenido**  **Estado**
  entrada**                                                     
  -------------- ---------------------- ----------------------- ------------
  Mínimo - 1: 0  Rechazado: no se       Se guarda y el perfil   Rechazado
  materias       guarda y el perfil     sigue siendo público    
  (quitas la     conserva su materia                            
  única materia) anterior                                       

  Mínimo: 1      Aceptado: mensaje \"Tu Se muestra mensaje \"Tu Aceptado
  materia        perfil académico y     perfil académico y      
                 materias se han        materias se han         
                 guardado               guardado                
                 exitosamente.\"        exitosamente.\"         

  Mínimo + 1: 2  Aceptado: mensaje \"Tu Se muestra mensaje \"Tu Aceptado
  materias       perfil académico y     perfil académico y      
                 materias se han        materias se han         
                 guardado               guardado                
                 exitosamente.\"        exitosamente.\"         

  Máximo - 1: 4  Aceptado: mensaje \"Tu Se muestra mensaje \"Tu Aceptado
  materias       perfil académico y     perfil académico y      
                 materias se han        materias se han         
                 guardado               guardado                
                 exitosamente.\"        exitosamente.\"         

  Máximo: 5      Aceptado: mensaje \"Tu Se muestra mensaje \"Tu Aceptado
  materias       perfil académico y     perfil académico y      
                 materias se han        materias se han         
                 guardado               guardado                
                 exitosamente.\"        exitosamente.\"         

  Máximo + 1: 6  Rechazado: no se       Se muestra mensaje \"Tu Aceptado
  materias       guarda y el perfil     perfil académico y      
                 conserva las 5         materias se han         
                 materias anteriores    guardado                
                                        exitosamente.\"         
  --------------------------------------------------------------------------

**C1:**

![](media/image71.png){width="6.1375in" height="3.452777777777778in"}

![](media/image72.png){width="6.1375in" height="3.452777777777778in"}

**C2:**

![](media/image73.png){width="6.1375in" height="3.452777777777778in"}

![](media/image74.png){width="6.1375in" height="3.452777777777778in"}

**C3:**

![](media/image75.png){width="6.1375in" height="3.452777777777778in"}

![](media/image76.png){width="6.1375in" height="3.452777777777778in"}

**C4:**

![](media/image77.png){width="6.1375in" height="3.452777777777778in"}

![](media/image78.png){width="6.1375in" height="3.452777777777778in"}

**C5:**

![](media/image79.png){width="6.1375in" height="3.452777777777778in"}

![](media/image80.png){width="6.1375in" height="3.452777777777778in"}

**C6:**

![](media/image81.png){width="6.1375in" height="3.452777777777778in"}

![](media/image82.png){width="6.1375in" height="3.452777777777778in"}

**Caso de prueba: CP-10 (RF-10 Eliminar perfil público del tutor)**

-   **Nombre:** Validación de valores límite de la cantidad de perfiles
    públicos al eliminar.

-   **Objetivo:** Verificar que el sistema permita eliminar el perfil
    cuando existe y no ofrezca borrar cuando no hay ninguno.

-   **Precondiciones:** Tener sesión iniciada como tutor aprobado. Para
    la primera fila, usa un tutor sin perfil creado; para la segunda,
    uno con perfil.

-   **Rango válido:** 1 perfil público existente.

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra a la configuración del perfil público, elige
    \"Eliminar Perfil\" y confirma. Después busca al tutor en el
    catálogo.

  --------------------------------------------------------------------------
  **Dato de        **Resultado esperado**        **Resultado    **Estado**
  entrada**                                      obtenido**     
  ---------------- ----------------------------- -------------- ------------
  Mínimo - 1: 0    Rechazado: no hay perfil que  No permite     Rechazado
  perfiles (tutor  eliminar y no sale ningún     eliminar el    
  sin perfil)      error                         perfil         

  Mínimo: 1        Aceptado: mensaje \"Perfil    No permite     Rechazado
  perfil, lo       eliminado exitosamente\" y el eliminar el    
  eliminas y       tutor ya no aparece en el     perfil         
  confirmas        catálogo                                     
  --------------------------------------------------------------------------

**Módulo 4: Horarios y catálogo de tutorías (RF-11 a RF-14)**

**Caso de prueba: CP-11 (RF-11 Registrar horarios de atención)**

-   **Nombre:** Validación de valores límite de la hora de un horario
    nuevo frente a uno ya registrado.

-   **Objetivo:** Verificar que el sistema acepte horarios que empiezan
    o terminan justo donde acaba o empieza otro, y rechace los que se
    empalman aunque sea por un minuto.

-   **Precondiciones:** Tener sesión iniciada como tutor aprobado con
    perfil público. Tener un único horario registrado el lunes de 10:00
    a 12:00. Después de cada prueba aceptada, elimina ese horario nuevo
    (CP-13) para que no estorbe a la siguiente. Si el sistema solo
    permite horas completas, usa los valores equivalentes (por ejemplo
    11:00 y 12:00).

-   **Rango válido:** Un horario nuevo el lunes que empiece a las 12:00
    o después, o que termine a las 10:00 o antes (sin empalme con 10:00
    a 12:00).

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra a gestión de horarios, elige lunes, escribe
    la hora de inicio y fin, elige un salón y presiona \"Guardar
    Horario\".

  ---------------------------------------------------------------------------
  **Dato de     **Resultado esperado**  **Resultado obtenido**   **Estado**
  entrada**                                                      
  ------------- ----------------------- ------------------------ ------------
  Mínimo - 1:   Rechazado: mensaje \"El Se muestra el mensaje    Aceptado
  lunes de      horario seleccionado se \"El horario             
  11:59 a 13:00 cruza con otro          seleccionado se cruza    
                existente\"             con otro existente\"     

  Mínimo: lunes Aceptado: mensaje       Se muestra el mensaje    Aceptado
  de 12:00 a    \"Horario registrado    \"Horario registrado     
  13:00         exitosamente\"          exitosamente\"           

  Mínimo + 1:   Aceptado: mensaje       Se muestra el mensaje    Aceptado
  lunes de      \"Horario registrado    \"Horario registrado     
  12:01 a 13:00 exitosamente\"          exitosamente\"           

  Máximo - 1:   Aceptado: mensaje       Se muestra el mensaje    Aceptado
  lunes de      \"Horario registrado    \"Horario registrado     
  09:00 a 09:59 exitosamente\"          exitosamente\"           

  Máximo: lunes Aceptado: mensaje       Se muestra el mensaje    Aceptado
  de 09:00 a    \"Horario registrado    \""Horario registrado    
  10:00         exitosamente\"          exitosamente\"           

  Máximo + 1:   Rechazado: mensaje \"El Se muestra el mensaje    Aceptado
  lunes de      horario seleccionado se Horario registrado       
  09:00 a 10:01 cruza con otro          exitosamente\"           
                existente\"                                      
  ---------------------------------------------------------------------------

**C1:**

![](media/image83.png){width="6.145833333333333in"
height="3.4583333333333335in"}

![](media/image84.png){width="6.145833333333333in"
height="3.4583333333333335in"}![](media/image85.png){width="6.145833333333333in"
height="3.4583333333333335in"}

C2:

![](media/image86.png){width="6.145833333333333in"
height="3.4583333333333335in"}![](media/image87.png){width="6.145833333333333in"
height="3.4583333333333335in"}

![](media/image88.png){width="6.145833333333333in"
height="3.4583333333333335in"}

C3:

![](media/image89.png){width="6.145833333333333in"
height="3.4583333333333335in"}![](media/image90.png){width="6.145833333333333in"
height="3.4583333333333335in"}![](media/image91.png){width="6.145833333333333in"
height="3.4583333333333335in"}

C4

![](media/image92.png){width="6.145833333333333in"
height="3.4583333333333335in"}![](media/image93.png){width="6.145833333333333in"
height="3.4583333333333335in"}![](media/image94.png){width="6.145833333333333in"
height="3.4583333333333335in"}

C5:

![](media/image95.png){width="6.145833333333333in"
height="3.4583333333333335in"}![](media/image96.png){width="6.145833333333333in"
height="3.4583333333333335in"}![](media/image97.png){width="6.145833333333333in"
height="3.4583333333333335in"}

C6:

![](media/image98.png){width="6.1375in" height="3.452777777777778in"}

![](media/image99.png){width="6.1375in" height="3.452777777777778in"}

![](media/image100.png){width="6.1375in" height="3.452777777777778in"}

**Caso de prueba: CP-12 (RF-12 Modificar horarios de atención)**

-   **Nombre:** Validación de valores límite de la hora de inicio al
    modificar un horario.

-   **Objetivo:** Verificar que el sistema acepte la nueva hora de
    inicio dentro del rango y rechace las que se empalman con otro
    horario o dejan el horario sin duración.

-   **Precondiciones:** Tener sesión iniciada como tutor aprobado con
    dos horarios el lunes: A de 10:00 a 12:00 y B de 13:00 a 14:00. En
    todas las pruebas solo cambias la hora de inicio de B (la hora de
    fin se queda en 14:00). Si el sistema pide una duración mínima mayor
    a 1 minuto, ajusta el último valor del rango.

-   **Rango válido:** Hora de inicio de B de 12:00 a 13:59.

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra a gestión de horarios, elige el horario B,
    cambia la hora de inicio y presiona \"Guardar Cambios\".

  --------------------------------------------------------------------------
  **Dato de         **Resultado           **Resultado           **Estado**
  entrada**         esperado**            obtenido**            
  ----------------- --------------------- --------------------- ------------
  Mínimo - 1:       Rechazado: mensaje de Se muestra mensaje de Aceptado
  inicio de B a las error por empalme con error por empalme con 
  11:59             el horario A          el horario A          

  Mínimo: inicio de Aceptado: mensaje     Se muestra mensaje    Aceptado
  B a las 12:00     \"Horario actualizado \"Horario actualizado 
                    exitosamente\"        exitosamente\"        

  Mínimo + 1:       Aceptado: mensaje     Se muestra mensaje    Aceptado
  inicio de B a las \"Horario actualizado \"Horario actualizado 
  12:01             exitosamente\"        exitosamente\"        

  Máximo - 1:       Aceptado: mensaje     Se muestra mensaje    Aceptado
  inicio de B a las \"Horario actualizado \"Horario actualizado 
  13:58             exitosamente\"        exitosamente\"        

  Máximo: inicio de Aceptado: mensaje     Se muestra mensaje    Aceptado
  B a las 13:59     \"Horario actualizado \"Horario actualizado 
                    exitosamente\"        exitosamente\"        

  Máximo + 1:       Rechazado: mensaje de Se muestra mensaje de Rechazado
  inicio de B a las error por horario     error por horario     
  14:00 (igual a la inválido              inválido              
  hora de fin)                                                  
  --------------------------------------------------------------------------

**C1:**

![](media/image101.png){width="6.1375in" height="3.452777777777778in"}

![](media/image102.png){width="6.1375in" height="3.452777777777778in"}

![](media/image103.png){width="6.1375in" height="3.452777777777778in"}

**C2:**

![](media/image101.png){width="6.1375in" height="3.452777777777778in"}

![](media/image104.png){width="6.1375in" height="3.452777777777778in"}

![](media/image105.png){width="6.1375in" height="3.452777777777778in"}

**C3:**

![](media/image101.png){width="6.1375in" height="3.452777777777778in"}

![](media/image106.png){width="6.1375in" height="3.452777777777778in"}

![](media/image107.png){width="6.1375in" height="3.452777777777778in"}

**C4:**

![](media/image101.png){width="6.1375in" height="3.452777777777778in"}

![](media/image108.png){width="6.1375in" height="3.452777777777778in"}

![](media/image109.png){width="6.1375in"
height="3.452777777777778in"}**C5:**

![](media/image101.png){width="6.1375in" height="3.452777777777778in"}

![](media/image110.png){width="6.1375in" height="3.452777777777778in"}

![](media/image111.png){width="6.1375in" height="3.452777777777778in"}

**C6:**

![](media/image101.png){width="6.1375in" height="3.452777777777778in"}

![](media/image112.png){width="6.1375in" height="3.452777777777778in"}

![](media/image113.png){width="6.1375in" height="3.452777777777778in"}

**Caso de prueba: CP-13 (RF-13 Eliminar horarios)**

-   **Nombre:** Validación de valores límite de la cantidad de horarios
    registrados al eliminar.

-   **Objetivo:** Verificar que el sistema permita eliminar un horario
    cuando hay uno o varios, borrando solo el elegido, y que no haya
    errores cuando no hay ninguno.

-   **Precondiciones:** Tener sesión iniciada como tutor aprobado con
    perfil público. Prepara la cantidad de horarios indicada en cada
    fila.

-   **Rango válido:** 1 o más horarios registrados.

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra a gestión de horarios, elige el horario a
    borrar, presiona eliminar y confirma en la ventana.

  -------------------------------------------------------------------------
  Dato de        Resultado esperado      Resultado obtenido      Estado
  entrada                                                        
  -------------- ----------------------- ----------------------- ----------
  Mínimo - 1: 0  La lista aparece vacía, La lista aparece vacía, Aceptado
  horarios       no hay nada que         no hay nada que         
  registrados    eliminar y no sale      eliminar y no sale      
                 ningún error            ningún error            

  Mínimo: 1      Aceptado: mensaje       Se muestra mensaje      Aceptado
  horario, lo    \"Registro de horario   \"Registro de horario   
  eliminas       eliminado               eliminado               
                 exitosamente\" y la     exitosamente\" y la     
                 lista queda vacía       lista queda vacía       

  Mínimo + 1: 2  Aceptado: se borra solo Se muestra mensaje      Aceptado
  horarios,      el elegido y el otro    \"Registro de horario   
  eliminas solo  sigue en la lista y en  eliminado               
  uno            el catálogo             exitosamente\" y la     
                                         lista queda vacía       
  -------------------------------------------------------------------------

**C1:**

![](media/image114.png){width="6.1375in" height="3.452777777777778in"}

**C2:**

![](media/image115.png){width="6.1375in" height="3.452777777777778in"}

![](media/image116.png){width="6.1375in" height="3.452777777777778in"}

**\
**

![](media/image117.png){width="6.1375in" height="3.452777777777778in"}

**C3:**

![](media/image118.png){width="6.1375in" height="3.452777777777778in"}

![](media/image119.png){width="6.1375in" height="3.452777777777778in"}

![](media/image120.png){width="6.1375in" height="3.452777777777778in"}

**Caso de prueba: CP-14 (RF-14 Consultar tutorías disponibles)**

-   **Nombre:** Validación de valores límite de la cantidad de
    resultados de la búsqueda de tutorías.

-   **Objetivo:** Verificar que el catálogo muestre las tutorías cuando
    hay una o varias coincidencias, avise cuando no hay ninguna y
    responda en máximo 3 segundos.

-   **Precondiciones:** Tener sesión iniciada. Tener en el catálogo al
    menos 2 tutores de la misma materia y 1 tutor de otra materia
    distinta.

-   **Rango válido:** 0 o más resultados, mostrados en máximo 3
    segundos.

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Abre el catálogo, escribe o elige el filtro
    (materia, academia o profesor) y cronometra cuánto tarda en mostrar
    los resultados.

  --------------------------------------------------------------------------
  **Dato de entrada**     **Resultado         **Resultado       **Estado**
                          esperado**          obtenido**        
  ----------------------- ------------------- ----------------- ------------
  Mínimo - 1: búsqueda    Aviso \"Sin         Se muestra el     Aceptado
  sin coincidencias (por  resultados\", sin   aviso \"Sin       
  ejemplo \"zzzzz\")      errores, en máximo  resultados\", sin 
                          3 segundos          errores, en       
                                              máximo 3 segundos 

  Mínimo + 1: búsqueda    Aceptado: se        Se muestran las 2 Aceptado
  con 2 coincidencias (la muestran las 2      tutorías con día, 
  materia que dan 2       tutorías con día,   horario y         
  tutores)                horario y ubicación ubicación, en     
                                              máximo 3 segundos 

  Mínimo: búsqueda con 1  Aceptado: se        Se muestra 1      Aceptado
  sola coincidencia (la   muestra 1 tutoría   tutoría con su    
  materia que solo da un  con su día, horario día, horario y    
  tutor)                  y ubicación         ubicación         

  Máximo: sin ningún      Aceptado: se        Se muestra la     Aceptado
  filtro (todas las       muestra la lista    lista completa    
  tutorías)               completa                              
  --------------------------------------------------------------------------

**C1:**

![](media/image121.png){width="6.1375in" height="3.452777777777778in"}

**C2:**

![](media/image122.png){width="6.1375in" height="3.452777777777778in"}

**C3:**

![](media/image123.png){width="6.1375in" height="3.452777777777778in"}

**C4:**

![](media/image124.png){width="6.1375in" height="3.452777777777778in"}

**Módulo 5: Asistencias (RF-15 a RF-19)**

**Caso de prueba: CP-15 (RF-15 Registrar asistencia manual)**

-   **Nombre:** Validación de valores límite del largo del número de
    boleta al buscar un alumno.

-   **Objetivo:** Verificar que el sistema encuentre al alumno con una
    boleta del largo correcto y rechace boletas más cortas o más largas.

-   **Precondiciones:** Tener sesión iniciada como tutor con una sesión
    de tutoría activa o programada. Tener la boleta real de un alumno
    registrado (en los ejemplos se usa 2022350123; cámbiala por la
    real). Estos ejemplos suponen boletas de 10 dígitos.

-   **Rango válido:** 10 dígitos (supuesto).

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra al control de asistencia de la sesión,
    escribe la boleta en el buscador, selecciona al alumno y presiona
    registrar asistencia manual.

  ----------------------------------------------------------------------------
  **Dato de entrada**   **Resultado esperado**      **Resultado   **Estado**
                                                    obtenido**    
  --------------------- --------------------------- ------------- ------------
  Mínimo - 1: 9 dígitos Rechazado: mensaje de que   Por ejecutar  Pendiente
  (boleta real sin el   el alumno no fue encontrado               
  último dígito:                                                  
  202235012)                                                      

  Mínimo y Máximo: 10   Aceptado: mensaje           Por ejecutar  Pendiente
  dígitos (boleta real: \"Asistencia manual                       
  2022350123)           registrada con éxito,                     
                        pendiente de confirmación                 
                        por el alumno\"                           

  Máximo + 1: 11        Rechazado: mensaje de que   Por ejecutar  Pendiente
  dígitos (boleta real  el alumno no fue encontrado               
  más un dígito:                                                  
  20223501231)                                                    
  ----------------------------------------------------------------------------

**Caso de prueba: CP-16 (RF-16 Confirmar asistencia manual)**

-   **Nombre:** Validación de valores límite de la cantidad de
    asistencias pendientes de confirmar.

-   **Objetivo:** Verificar que el alumno pueda confirmar o rechazar una
    o varias asistencias pendientes, y que no haya errores cuando no
    tiene ninguna.

-   **Precondiciones:** Tener sesión iniciada como alumno. Haber
    registrado asistencias manuales a ese alumno con CP-15 según la
    cantidad de cada fila.

-   **Rango válido:** 1 o más asistencias pendientes.

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra a la sección de asistencias pendientes y
    elige \"Confirmar\" o \"Rechazar\" en cada registro.

  -------------------------------------------------------------------------
  Dato de entrada       Resultado esperado         Resultado    Estado
                                                   obtenido     
  --------------------- -------------------------- ------------ -----------
  Mínimo - 1: 0         Lista vacía y sin errores: Por ejecutar Pendiente
  asistencias           no hay nada que confirmar               
  pendientes                                                    

  Mínimo: 1 asistencia  Aceptado: mensaje          Por ejecutar Pendiente
  pendiente, la         \"Asistencia confirmada                 
  confirmas             exitosamente\" y el                     
                        registro queda como                     
                        confirmado                              

  Mínimo + 1: 2         Aceptado: una queda        Por ejecutar Pendiente
  asistencias           \"Confirmada\", la otra                 
  pendientes, confirmas \"Rechazada\" y el tutor                
  una y rechazas otra   recibe aviso de la                      
                        discrepancia                            
  -------------------------------------------------------------------------

**Caso de prueba: CP-17 (RF-17 Generar código QR dinámico)**

-   **Nombre:** Validación de valores límite de la hora al generar el QR
    frente al horario de atención.

-   **Objetivo:** Verificar que el tutor pueda generar el QR solo desde
    el minuto en que empieza su horario hasta el minuto en que termina.

-   **Precondiciones:** Tener sesión iniciada como tutor aprobado. Crea
    un horario de hoy de unos 10 minutos, con inicio pocos minutos
    después de la hora actual (por ejemplo, si son las 16:30, crea 16:35
    a 16:45). Las filas se prueban en orden, minuto a minuto, usando la
    hora de ese horario.

-   **Rango válido:** Desde la hora de inicio hasta la hora de fin del
    horario de atención (incluidas ambas).

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra al módulo de asistencia y presiona \"Generar
    Código QR de Asistencia\" en el minuto indicado. Fíjate en la hora
    del reloj de tu equipo.

  --------------------------------------------------------------------------
  Dato de entrada     Resultado esperado            Resultado    Estado
                                                    obtenido     
  ------------------- ----------------------------- ------------ -----------
  Mínimo - 1: 1       Rechazado: no deja generar el Por ejecutar Pendiente
  minuto antes de la  QR y avisa que estás fuera de              
  hora de inicio      horario                                    
  (16:34)                                                        

  Mínimo: a la hora   Aceptado: aparece el QR en    Por ejecutar Pendiente
  exacta de inicio    pantalla con su temporizador               
  (16:35)                                                        

  Mínimo + 1: 1       Aceptado: aparece el QR en    Por ejecutar Pendiente
  minuto después del  pantalla con su temporizador               
  inicio (16:36)                                                 

  Máximo - 1: 1       Aceptado: aparece el QR en    Por ejecutar Pendiente
  minuto antes del    pantalla con su temporizador               
  fin (16:44)                                                    

  Máximo: a la hora   Aceptado: aparece el QR en    Por ejecutar Pendiente
  exacta de fin       pantalla (si el sistema ya no              
  (16:45)             deja, anótalo como                         
                      observación)                               

  Máximo + 1: 1       Rechazado: no deja generar el Por ejecutar Pendiente
  minuto después del  QR y avisa que estás fuera de              
  fin (16:46)         horario                                    
  --------------------------------------------------------------------------

**Caso de prueba: CP-18 (RF-18 Registrar asistencia mediante QR)**

-   **Nombre:** Validación de valores límite del tiempo de vigencia del
    QR al escanearlo.

-   **Objetivo:** Verificar que el alumno pueda registrar su asistencia
    con el QR durante los 3 minutos de vigencia y que después el código
    ya no sirva.

-   **Precondiciones:** Tener sesión iniciada como alumno en un celular
    y al tutor con el QR recién generado en otra pantalla. Para cada
    fila, pide al tutor un QR nuevo, cronometra desde que aparece y usa
    un alumno distinto o un QR nuevo para que no cuente como asistencia
    repetida. El temporizador que muestra la pantalla del tutor te sirve
    de guía.

-   **Rango válido:** Desde que se genera el QR hasta 3 minutos (180
    segundos).

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Abre el escáner de QR en tu sesión de alumno y
    apunta la cámara al QR cuando el cronómetro marque el tiempo de la
    tabla.

  --------------------------------------------------------------------------
  Dato de entrada         Resultado esperado       Resultado     Estado
                                                   obtenido      
  ----------------------- ------------------------ ------------- -----------
  Mínimo: escanear lo más Aceptado: mensaje        Por ejecutar  Pendiente
  rápido posible después  \"Asistencia registrada                
  de generarlo            exitosamente\"                         

  Máximo - 1: escanear a  Aceptado: mensaje        Por ejecutar  Pendiente
  los 2 minutos 59        \"Asistencia registrada                
  segundos (179 s)        exitosamente\"                         

  Máximo: escanear a los  Aceptado: mensaje        Por ejecutar  Pendiente
  3 minutos exactos (180  \"Asistencia registrada                
  s)                      exitosamente\"                         

  Máximo + 1: escanear a  Rechazado: mensaje \"El  Por ejecutar  Pendiente
  los 3 minutos 1 segundo código QR ha expirado\"                
  (181 s)                                                        
  --------------------------------------------------------------------------

**Caso de prueba: CP-19 (RF-19 Visualizar historial de asistencias)**

-   **Nombre:** Validación de valores límite de la cantidad de registros
    en el historial de asistencias.

-   **Objetivo:** Verificar que el historial muestre correctamente uno o
    varios registros y que no tenga errores cuando está vacío.

-   **Precondiciones:** Tener un alumno y un tutor. Para cada fila, usa
    un alumno con la cantidad de asistencias indicada (confirmadas o
    registradas). Prueba cada fila con la sesión de alumno y también con
    la de tutor.

-   **Rango válido:** 0 o más registros.

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra al módulo de historial de asistencias y
    revisa lo que aparece.

  -------------------------------------------------------------------------
  Dato de entrada       Resultado esperado         Resultado    Estado
                                                   obtenido     
  --------------------- -------------------------- ------------ -----------
  Mínimo: 0 registros   Historial vacío y sin      Por ejecutar Pendiente
  (usuario nuevo sin    errores                                 
  asistencias)                                                  

  Mínimo + 1: 1         Aceptado: se muestra el    Por ejecutar Pendiente
  registro              registro con fecha, hora,               
                        materia, tutor y ubicación              

  Mínimo + 2: 2         Aceptado: se muestran los  Por ejecutar Pendiente
  registros             2 registros completos, sin              
                        mezclar ni repetir datos                
  -------------------------------------------------------------------------

**Módulo 6: Calificaciones, notificaciones y reportes (RF-20 a RF-24)**

**Caso de prueba: CP-20 (RF-20 Calificar tutoría)**

-   **Nombre:** Validación de valores límite de las estrellas al
    calificar una tutoría.

-   **Objetivo:** Verificar que el sistema acepte calificaciones de 0 a
    5 estrellas y no permita valores fuera de ese rango.

-   **Precondiciones:** Tener sesión iniciada como alumno con una
    asistencia confirmada por cada calificación que pruebes (si el
    sistema deja volver a calificar la misma sesión, puedes
    reutilizarla).

-   **Rango válido:** 0 a 5 estrellas.

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra al historial de asistencias, elige la sesión
    y selecciona las estrellas indicadas. Si la pantalla no te deja
    elegir -1 ni 6, eso cuenta como \"Rechazado\" y es lo correcto.

  -------------------------------------------------------------------------
  Dato de        Resultado esperado              Resultado      Estado
  entrada                                        obtenido       
  -------------- ------------------------------- -------------- -----------
  Mínimo - 1: -1 Rechazado: no se puede          Por ejecutar   Pendiente
  estrellas      seleccionar o no se guarda la                  
                 calificación                                   

  Mínimo: 0      Aceptado: la calificación se    Por ejecutar   Pendiente
  estrellas      guarda                                         

  Mínimo + 1: 1  Aceptado: la calificación se    Por ejecutar   Pendiente
  estrella       guarda                                         

  Máximo - 1: 4  Aceptado: la calificación se    Por ejecutar   Pendiente
  estrellas      guarda                                         

  Máximo: 5      Aceptado: la calificación se    Por ejecutar   Pendiente
  estrellas      guarda                                         

  Máximo + 1: 6  Rechazado: no se puede          Por ejecutar   Pendiente
  estrellas      seleccionar o no se guarda la                  
                 calificación                                   
  -------------------------------------------------------------------------

**Caso de prueba: CP-21 (RF-21 Visualizar indicadores de
calificaciones)**

-   **Nombre:** Validación de valores límite del promedio global de
    calificaciones.

-   **Objetivo:** Verificar que el promedio mostrado sea correcto en los
    extremos de la escala (0 y 5) y que no haya errores cuando todavía
    no hay calificaciones.

-   **Precondiciones:** Tener sesión iniciada como Coordinación. Usa
    tutores distintos y filtra por profesor en cada fila para que no se
    mezclen las calificaciones. Prepara las calificaciones con CP-20.

-   **Rango válido:** Promedio de 0 a 5.

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra al módulo de estadísticas e indicadores,
    filtra por el profesor de cada fila y revisa el promedio que
    aparece.

  -------------------------------------------------------------------------
  Dato de entrada      Resultado esperado          Resultado    Estado
                                                   obtenido     
  -------------------- --------------------------- ------------ -----------
  Mínimo - 1: tutor    Sin promedio o aviso de que Por ejecutar Pendiente
  sin ninguna          no hay datos, sin errores y              
  calificación         sin mostrar un número                    
                       negativo                                 

  Mínimo: tutor con    Aceptado: el promedio       Por ejecutar Pendiente
  una calificación de  mostrado es 0                            
  0                                                             

  Mínimo + 1: tutor    Aceptado: el promedio       Por ejecutar Pendiente
  con calificaciones   mostrado es 0.5                          
  de 0 y 1                                                      

  Máximo - 1: tutor    Aceptado: el promedio       Por ejecutar Pendiente
  con calificaciones   mostrado es 4.5                          
  de 4 y 5                                                      

  Máximo: tutor con    Aceptado: el promedio       Por ejecutar Pendiente
  una calificación de  mostrado es 5                            
  5                                                             
  -------------------------------------------------------------------------

**Caso de prueba: CP-22 (RF-22 Enviar notificaciones)**

-   **Nombre:** Validación de valores límite del largo del mensaje de
    una notificación.

-   **Objetivo:** Verificar que el sistema envíe mensajes dentro del
    largo permitido y rechace los vacíos o demasiado largos.

-   **Precondiciones:** Tener sesión iniciada como tutor aprobado o
    Coordinación, y seleccionar al menos un alumno como destinatario en
    cada prueba. Para los textos largos, usa la letra \"a\" repetida y
    un contador de caracteres en línea para comprobar el largo exacto.

-   **Rango válido:** 1 a 500 caracteres (supuesto).

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra al módulo de avisos, escribe el mensaje con
    el largo de la tabla, elige al alumno y presiona \"Enviar\".

  --------------------------------------------------------------------------
  Dato de entrada     Resultado esperado           Resultado     Estado
                                                   obtenido      
  ------------------- ---------------------------- ------------- -----------
  Mínimo - 1: 0       Rechazado: mensaje de error  Por ejecutar  Pendiente
  caracteres (mensaje por campos obligatorios y no               
  vacío)              se envía                                   

  Mínimo: 1 carácter  Aceptado: mensaje            Por ejecutar  Pendiente
  (a)                 \"Notificación enviada con                 
                      éxito\"                                    

  Mínimo + 1: 2       Aceptado: mensaje            Por ejecutar  Pendiente
  caracteres (aa)     \"Notificación enviada con                 
                      éxito\"                                    

  Máximo - 1: 499     Aceptado: mensaje            Por ejecutar  Pendiente
  caracteres          \"Notificación enviada con                 
                      éxito\"                                    

  Máximo: 500         Aceptado: mensaje            Por ejecutar  Pendiente
  caracteres          \"Notificación enviada con                 
                      éxito\"                                    

  Máximo + 1: 501     Rechazado: mensaje de que se Por ejecutar  Pendiente
  caracteres          superó el largo permitido y                
                      no se envía                                
  --------------------------------------------------------------------------

**Caso de prueba: CP-23 (RF-23 Acceder a bandeja de entrada)**

-   **Nombre:** Validación de valores límite de la cantidad de
    notificaciones en la bandeja de entrada.

-   **Objetivo:** Verificar que la bandeja muestre bien una o varias
    notificaciones, marque como \"Leída\" solo la que se abre, y avise
    cuando está vacía.

-   **Precondiciones:** Tener sesión iniciada con cualquier usuario.
    Para cada fila, usa un usuario que tenga la cantidad de
    notificaciones indicada (envíalas con CP-22).

-   **Rango válido:** 0 o más notificaciones.

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Haz clic en el icono de notificaciones, revisa la
    lista y abre una notificación.

  -------------------------------------------------------------------------
  Dato de entrada      Resultado esperado          Resultado    Estado
                                                   obtenido     
  -------------------- --------------------------- ------------ -----------
  Mínimo: 0            Aviso de que no hay         Por ejecutar Pendiente
  notificaciones       notificaciones nuevas o                  
                       registradas, sin errores                 

  Mínimo + 1: 1        Aceptado: aparece marcada   Por ejecutar Pendiente
  notificación         como pendiente de lectura                
                       y, al abrirla, cambia a                  
                       \"Leída\"                                

  Mínimo + 2: 2        Aceptado: la abierta pasa a Por ejecutar Pendiente
  notificaciones,      \"Leída\" y la otra sigue                
  abres solo una       pendiente de lectura                     
  -------------------------------------------------------------------------

**Caso de prueba: CP-24 (RF-24 Generar reportes estadísticos)**

-   **Nombre:** Validación de valores límite de la cantidad de registros
    disponibles para el reporte.

-   **Objetivo:** Verificar que el sistema genere el PDF cuando hay uno
    o varios registros, en máximo 10 segundos, y avise cuando no hay
    información.

-   **Precondiciones:** Tener sesión iniciada como Coordinación. Elige
    filtros (por ejemplo, un periodo de fechas) que dejen la cantidad de
    registros de cada fila; para 0 registros usa un periodo en el que no
    haya habido tutorías.

-   **Rango válido:** 1 o más registros, con el PDF listo en máximo 10
    segundos.

-   **Técnica:** Análisis de valores límite.

-   **Cómo probar:** Entra al módulo de reportes, elige los filtros,
    presiona \"Exportar a PDF\" y cronometra hasta que se descargue.
    Abre el archivo para revisar que no esté vacío ni dañado.

  -------------------------------------------------------------------------
  Dato de entrada  Resultado esperado         Resultado         Estado
                                              obtenido          
  ---------------- -------------------------- ----------------- -----------
  Mínimo - 1: 0    Rechazado: advertencia de  Se permite        Rechazado
  registros        que no hay información     descargar PDF     
  (periodo sin     suficiente y no se                           
  tutorías)        descarga ningún PDF                          

  Mínimo: 1        Aceptado: se descarga el   Se permite        Aceptado
  registro         PDF con ese registro       descargar el PDF  
                                              con ese registro  

  Mínimo + 1: 2    Aceptado: se descarga el   Se permite        Aceptado
  registros        PDF con los 2 registros    descargar el PDF  
                                              con ese registro  
  -------------------------------------------------------------------------

**C1:**

![](media/image125.png){width="6.1375in" height="3.452777777777778in"}

![](media/image126.png){width="6.1375in" height="3.452777777777778in"}

**C2:**

**C3:**
