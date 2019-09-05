using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsGoiaService
{
    static class Program
    {
        #region Variables Globales
        public static int CurrentCoop { get; set; }
        public static string CurrentUrl { get; set; }
        public static string CurrentUser { get; set; }
        public static string CurrentPass { get; set; }
        public static string CurrentToken { get; set; }
        public static int minutosTratamiento { get; set; }
        public static int minutosProduccion { get; set; }
        #endregion

        /// <summary>
        /// Punto de entrada principal para la aplicación. 
        /// Simplemente hace una llamada al constructor del Servicio de Windows.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new MainService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
