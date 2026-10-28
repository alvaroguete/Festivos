# Festivos de Colombia

Widget nativo de Windows 10/11 x64, C# y WPF sobre .NET 10. Funciona sin conexión. El ejecutable publicado incluye .NET; el usuario no necesita instalarlo.

## Uso

Extrae el ZIP y abre **FestivosColombia.exe** con doble clic. Conserva el archivo en una carpeta estable si activas el inicio con Windows. El primer arranque puede tardar mientras .NET extrae bibliotecas nativas al directorio temporal. El binario no está firmado: Windows puede mostrar la advertencia de reputación de SmartScreen.

- Mantén pulsado el botón izquierdo sobre la bandera, el texto Colombia o el espacio libre del encabezado y arrastra para mover la nota. Puedes hacerlo también con la opción siempre encima activada. Arrastra cualquier borde o esquina para cambiar el tamaño.
- « y » cambian el año; ‹ y › cambian el mes. Selecciona un año o escríbelo y pulsa Entrar (1583–9998).
- Domingos en rojo; festivos con fondo coral; hoy con borde azul verdoso. El tooltip indica el nombre; pulsa una fecha para ver el detalle completo.
- La bandera fija o libera la ventana por encima de las demás. − minimiza y × cierra.
- ⚙ permite elegir color, opacidad (55–100 %), inicio con Windows, siempre encima y volver al mes actual. Restaurar valores predeterminados restablece las opciones de esa ventana; Guardar las aplica.
- En menos de 530 unidades de altura se muestra el modo compacto; desde 530 aparecen los festivos del mes y desde 690 los próximos tres festivos a partir de hoy. Las medidas son unidades WPF, independientes de la escala de pantalla.

Las preferencias se guardan automáticamente en `%LOCALAPPDATA%\FestivosColombia\preferences.json`. El inicio automático utiliza `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`, sin permisos de administrador. Desactívalo antes de mover o eliminar el ejecutable. La posición se ajusta al área de trabajo de un monitor disponible al abrir y al cambiar la configuración de pantallas.

## Compilar y probar

Para desarrollar instala el SDK .NET 10. En Windows ejecuta:

```powershell
dotnet run --project tests/Tests.csproj -c Release
dotnet run --project tests/WindowsSmoke -c Release
./scripts/publish.ps1
```

El runner del motor es un ejecutable de pruebas sin dependencias externas: imprime cada comprobación, el total y devuelve un código distinto de cero si falla. No se ejecuta con `dotnet test`.

Publicación equivalente:

```powershell
dotnet publish src/App/App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o artifacts/publish
```

Resultados: `artifacts/publish/FestivosColombia.exe` y `artifacts/FestivosColombia.zip` (ejecutable, fuentes, pruebas e instrucciones).

En GitHub, el flujo **Compilar y validar Windows** se ejecuta con push/PR o manualmente desde Actions. Al completarse ofrece el artefacto **FestivosColombia-Windows-x64**. El flujo necesita estar publicado en el repositorio; crear el archivo local no lo ejecuta en GitHub.

## Estructura y reglas

`src/Core` contiene el motor puro, navegación y persistencia; `src/App` contiene la vista WPF, las preferencias y la integración Windows. La vista usa notificación de propiedades para sus enlaces y eventos para las interacciones de ventana. No hay servicios externos, telemetría ni peticiones de red en la aplicación.

Pascua se calcula con Meeus/Jones/Butcher gregoriano. Se contemplan las 18 celebraciones nacionales, los siete traslados de fecha de calendario y Ascensión, Corpus Christi y Sagrado Corazón. Las fechas fijas permanecen en domingo cuando corresponde. Dos celebraciones pueden compartir descanso legal: se conservan ambos nombres; por ello no siempre existen 18 días distintos. Se aplica el calendario legal vigente de forma proyectada, no una reconstrucción histórica de las normas anteriores a 1983.

Ver `VALIDACION.md` para el alcance real y la revisión visual pendiente en Windows.
