using WindowsGoiaService.APIService;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsGoiaService.Jobs.Tratamiento
{
    public class TratamientoJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            dynamic result = null;

            // *******************************************************************************************************
            // PASO 1: INTENTAMOS OBTENER UN TOKEN VALIDO
            // *******************************************************************************************************

            Log.Information(".........Comienza la ejecución de la consulta sobre los tratamientos activos...........................OK");

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

            // *******************************************************************************************************
            // PASO 2: CONSULTAMOS LA LISTA DE TRATAMIENTOS ACTIVOS
            // *******************************************************************************************************

            APIConnection.setToken(Program.CurrentToken);
            result = await APIConnection.GetAsync("api/tratamiento/getall?cultivo=1");

            // Comprobamos el estado de retorno de la consulta
            if (APIConnection.status == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<WindowsGoiaService.Models.TratamientoViewModel> misTratamientos = new List<Models.TratamientoViewModel>();

                    // Convertimos los datos devueltos en un vector de elementos del tipo Json
                    string datos = Convert.ToString(result.data);
                    Newtonsoft.Json.Linq.JArray tratamientos = Newtonsoft.Json.Linq.JArray.Parse(datos) as Newtonsoft.Json.Linq.JArray;

                    // Bucle para recorrer cada elemento del vector devuelto
                    foreach (var item in tratamientos)
                    {
                        // Definimos el tipo del objeto json
                        Newtonsoft.Json.Linq.JObject jtratamiento = item as Newtonsoft.Json.Linq.JObject;

                        // Almacenamos los resultados en un listado
                        WindowsGoiaService.Models.TratamientoViewModel tratamiento = jtratamiento.ToObject<WindowsGoiaService.Models.TratamientoViewModel>();
                        misTratamientos.Add(tratamiento);
                    }

                    if (misTratamientos.Count > 0)
                    {
                        //**********************************************************************************
                        //
                        //              PROGAMACIÓN INTERNA CON LOS TRATAMIENTOS CONSULTADOS
                        //
                        //**********************************************************************************
                    }
                }
                catch (Exception excp)
                {
                    Log.Error("No ha sido posible deserializar los resultados obtenidos. Motivo: " + excp.Message);
                }
            }
            else
                Log.Error("No ha sido posible consultar el listado de tratamientos activos. Por favor, revise que el servicio remoto está operativo.");
        }
    }
}
