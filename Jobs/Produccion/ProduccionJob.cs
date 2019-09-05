using WindowsGoiaService.APIService;
using WindowsGoiaService.Models;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsGoiaService.Jobs.Produccion
{
    public class ProduccionJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            dynamic result = null;

            // *******************************************************************************************************
            // PASO 1: INTENTAMOS OBTENER UN TOKEN VALIDO
            // *******************************************************************************************************

            Log.Information(".........Comienza la ejecución de la consulta sobre los vales de produccion pendientes...........................OK");

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
            // PASO 2: INTENTAMOS INSERTAR UN NUEVO VALE DE PRODUCCION EN GOIA
            // *******************************************************************************************************

            APIConnection.setToken(Program.CurrentToken);

            Log.Information(".........Intentamos insertar el número de vale Nº 1.034.089...........................OK");
            ProduccionViewModel nuevoVale = new ProduccionViewModel();
            nuevoVale.cultivo = 1;
            nuevoVale.numvale = "1.034.089";
            nuevoVale.fecha = Convert.ToDateTime("01/01/2000");
            nuevoVale.codfinca = "38005F13";
            nuevoVale.codparcela = "1";
            nuevoVale.kilos = 406;
            nuevoVale.unidades = 10;

            result = await APIConnection.PostAsync("api/produccion/postinserta", JsonConvert.SerializeObject(nuevoVale));

            if (APIConnection.status == System.Net.HttpStatusCode.OK)
            {
                //**********************************************************************************
                //
                //              PROGAMACIÓN INTERNA UNA VEZ INSERTADO EL VALE DE PRODUCCION
                //
                //**********************************************************************************
            }
            else
            {
                Log.Error("No se ha podido insertar el vale de producción Nº: " + nuevoVale.numvale.ToString() + " en la base de datos de GOIA.");
            }

        }
    }
}
