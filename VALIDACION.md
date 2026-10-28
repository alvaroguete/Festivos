# Validación

## Automatización

El runner `tests/Tests.csproj` compara listas explícitas de las 18 celebraciones para 2025, 2026, 2027, 2030 y 2040; fechas de Pascua; traslados de los siete días de semana; navegación entre años; bisiesto; domingo fijo; límites de navegación; persistencia, JSON corrupto y normalización.

`tests/WindowsSmoke` abre WPF en un hilo STA, fuerza cinco tamaños (incluida la vuelta a compacto), procesa el dispatcher y comprueba 42 casillas, altura útil y visibilidad del modo. GitHub Actions también inicia el EXE publicado y comprueba que aparece una ventana y sigue en ejecución. Estos checks no sustituyen la inspección visual.

## Revisión manual necesaria en Windows

En Windows 10 y 11, repetir con escala 100 %, 125 % y 150 %:

1. Abrir por doble clic sin instalar .NET, desconectar internet y recorrer los años de referencia.
2. Redimensionar por los cuatro bordes y cuatro esquinas; verificar texto legible, encabezado sin superposición y calendario completo en los tres modos.
3. Revisar tooltip y selección de festivos, fecha actual y dos celebraciones coincidentes (30/06/2025).
4. Cambiar opacidad y color, fijar, minimizar y restaurar; cerrar y reabrir y verificar preferencias.
5. Activar inicio automático y volver a iniciar sesión; desactivarlo y comprobar su retirada.
6. Mover a otro monitor, cerrar y desconectar ese monitor; comprobar recuperación visible. Repetir con monitores de escalas distintas.

Linux permite ejecutar las pruebas del motor y compilar/publicar para Windows, pero no ejecutar WPF ni certificar visualmente las escalas. No se afirma que esa revisión manual se haya realizado.

## Resultado de esta entrega (28 de octubre de 2026)

- SDK 10.0.403 en Linux x64.
- 28 comprobaciones del motor, navegación y preferencias ejecutadas y superadas.
- Publicación Windows x64 self-contained y single-file completada.
- Compilación de la prueba WPF completada sin advertencias ni errores.
- Ejecutable identificado como PE32+ GUI x86-64.
- No se ejecutó WPF en esta máquina Linux. El flujo GitHub Actions está incluido, pero no se ha enviado ni ejecutado remotamente. La revisión manual de Windows y DPI queda pendiente.
