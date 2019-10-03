# GoiaService
Servicio windows para la ejecución automática y periódica de llamadas a la API de GOIA


## Comenzando

Este código de ejemplo puede servir como punto de partida para el desarrollo, configuración e instalación de un servicio windows capaz de ejecutarse en cualquier equipo, para el intercambio de información entre la base de datos local de una cooperativa, empaquetado o empresa comercializadora y los servicios API de la plataforma GOIA.


### Pre-requisitos 📋

Tener instalado Visual Studio 2015 o superior, con .Net Framework 2.5.4 o superior y conocimientos en el lenguaje c#.


### Parametrización ⚙️

En el fichero **App.config** encontraremos todos los parámetros necesarios para configurar nuestro servicio, tales como:

- **urlAPIService** : La dirección donde se encuentra publicada la API de GOIA.
- **userAPIService** : Email del usuario con acceso al módulo **GOIA - Cooperativas**.
- **passAPIService** : Contraseña del usuario con acceso al módulo **GOIA - Cooperativas**.
- **coopAPIService** : El identificador único de la cooperativa al que nos queremos conectar.
- **minutosTratamiento** : Los minutos entre cada ejecución del servicio para obtener los tratamientos activos.
- **minutosProduccion** : Los minutos entre cada ejecución del servicio para enviar los datos de producción.

Tal y como está programado actualmente, estos parámetros son leídos y cargados en memoria durante la inicialización del servicio (línea nº 98 de archivo MainService.cs). Por tanto, en caso de que se modifique algún valor, es necesario reiniciar el servicio para que dichos cambios surtan efecto.

### Antes de cualquier llamada

Cada vez que se intente invocar a cualquier servicio de GOIA (por ejemplo, ProduccionJob.cs o TratamientoJob.cs), es necesario ejecutar este código para garantizar que disponemos de un token válido para establecer la conexión. En caso de que no sea así, el sistema intentará obtener uno nuevo a partir del correo electrónico y contraseña especificadas en el fichero de configuración. 

Por tanto, este código debe incluirse en cualquier servicio adicional que quiera añadirse.

```
WebApiClient APIConnection = new WebApiClient(Program.CurrentUrl, Program.CurrentToken);

            result = await APIConnection.tryLoginWithToken(Program.CurrentToken);

            // Comprobamos si el token que teníamos almacenado en la variable <<Program.CurrentToken>> sigue siendo válido
            if (APIConnection.status == System.Net.HttpStatusCode.Unauthorized)
            {
                result = await APIConnection.Login(Program.CurrentUser, Program.CurrentPass, Program.CurrentCoop);

                if (APIConnection.status == System.Net.HttpStatusCode.BadRequest)
                {
                    Log.Error("No ha sido posible obtener un token válido con los parámetros especificados {param1}/{param2}/{param3} ", Program.CurrentCoop, Program.CurrentUser, Program.CurrentPass);
                    return;
                }
                else
                {
                    Program.CurrentToken = result.access_token;
                }
            }
```

En caso de que no se haya podido obtener un nuevo token válido, el proceso registra el error en el visor de sucesos de windows y termina su ejecución.

### La llamada

Una vez obtenido el token válido del paso anterior, ya estamos listos para ejecutar la llamada a la API de GOIA, pasándole como parámetro en la cabecera de la petición dicho token. Veámos algunos ejemplos:

En caso de una consulta de datos, lo haremos mediante una petición GET:
´´´
            APIConnection.setToken(Program.CurrentToken);
            result = await APIConnection.GetAsync("api/tratamiento/getall?cultivo=1");
´´´

En caso de envío de datos, lo haremos mediante una petición POST:
´´´
            result = await APIConnection.PostAsync("api/produccion/postinserta", JsonConvert.SerializeObject(nuevoVale));
´´´
 
## Instalación 🔧

En este apartado se explica cómo podemos desplegar este servicio en cualquier máquina con Sistema Operativo Windows. Para obtener el servicio es necesario que la solución compile correctamente. 

Si el sistema no encuentra installutil.exe, asegúrese de que existe en el equipo. Esta herramienta se instala con .NET Framework en la carpeta %windir%\Microsoft.NET\Framework[64]\<[versión de Framework]> . 
Por ejemplo, la ruta de acceso predeterminada para la versión de 64 bits es %windir%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe.

Nos cambiamos al directorio donde se encuentra instalado el .NET Framework

```
cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319
```

Instalamos el servicio

```
InstallUtil.exe /u C:\{ruta_del_release_del_proyecto}\WindowsGoiaService.exe
```

Desinstalamos el servicio

```
InstallUtil.exe C:\{ruta_del_release_del_proyecto}\WindowsGoiaService.exe
```
