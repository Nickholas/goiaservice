# GoiaService
Servicio windows para la ejecución automática y periódica de llamadas a la API de GOIA

## Introducción
Este código permite la configuración e instalación de un servicio windows quepuede ejecutarse en cualquier equipo para intercambiar información entre la base de datos local de una cooperativa, empaquetado o empresa comercializadora con los servicios API de la plataforma GOIA

## Comenzando

Código de ejemplo para construir un servicio que se ejecute en un entorno Windows para comunicarse periódicamente con los servicios web de GOIA (API) y obtener así la información para facilitar la integración con cualquier otra aplicación


### Pre-requisitos 📋

Tener instalado Visual Studio 2015 o superior, con .Net Framework 2.5.4 o superior y conocimientos en el lenguaje c#.


### Instalación 🔧

Si el sistema no encuentra installutil.exe, asegúrese de que existe en el equipo. Esta herramienta se instala con .NET Framework en la carpeta %windir%\Microsoft.NET\Framework[64]\<[versión de Framework]> . 
Por ejemplo, la ruta de acceso predeterminada para la versión de 64 bits es %windir%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe.

Nos cambiamos al directorio donde se encuentra instalado el .NET Framework

```
cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319
```

Instalamos el servicio

```
InstallUtil.exe /u C:\{ruta_del_release_del_proyecto}\CoplacaWinService.exe
```

Desinstalamos el servicio

```
InstallUtil.exe C:\{ruta_del_release_del_proyecto}\CoplacaWinService.exe
```
## Ejecutando las pruebas ⚙️

En el fichero **App.config** encontraremos todos los parámetros necesarios para configurar nuestro servicio, tales como el usuario y password de GOIA para conectarse y obtener un token válido, los minutos entre cada ejecución del servicio, etc.
