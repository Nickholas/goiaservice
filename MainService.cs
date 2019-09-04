using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WindowsGoiaService.Jobs.Produccion;
using WindowsGoiaService.Jobs.Tratamiento;

namespace WindowsGoiaService
{
    public partial class MainService : ServiceBase
    {
        #region Variables Privadas
        private enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        private TratamientoJobScheduler tratamientoJobScheduler;
        private ProduccionJobScheduler produccionJobScheduler;

        private int eventId;
        #endregion

        public MainService()
        {
            InitializeComponent();

            //Setup Service
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = false;

            //Setup logging
            this.AutoLog = false;

            this.eventLogService = new EventLog();
            this.eventLogService.Source = "CoplacaLog";
            this.eventLogService.Log = "Application";

            if (!System.Diagnostics.EventLog.SourceExists("CoplacaLog"))
                System.Diagnostics.EventLog.CreateEventSource(this.eventLogService.Source, this.eventLogService.Log);

            //Inicializamos el identificador de los eventos
            this.eventId = 1;

            //Creamos el registro de sucesos en el subdirectorio /logs
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\coplaca-.log", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            //***************************************************************************
            //INICIALIZAMOS LAS TAREAS A REALIZAR
            //***************************************************************************
            this.tratamientoJobScheduler = new TratamientoJobScheduler();
            this.produccionJobScheduler = new ProduccionJobScheduler();
        }

        #region Funciones Protegidas propias del servicio

        //Durante el arranque, escribimos la operación en el visor
        //de sucesos y especificamos lo que vamos a realizar. También 
        //ponemos el servicio en estado "Pendiente de ejecución..."
        protected override void OnStart(string[] args)
        {
            this.eventLogService.WriteEntry("Intentando iniciar el servicio CoplacaService....");

            //Comprobamos que se cumplen los pre-requisitos necesarios
            string prerequisitos = CompruebaPreRequisitos();
            if (!string.IsNullOrEmpty(prerequisitos))
            {
                Log.Error(prerequisitos);
                this.eventLogService.WriteEntry(prerequisitos);
                return;
            }
            else
                Log.Information("Se ha cumplido correctamente todos los Pre-requisitos...........................OK");

            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            //serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            //serviceStatus.dwWaitHint = 100000;
            //SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            //***************************************************************************
            //COMENZAMOS LAS TAREAS A EJECUTAR
            //***************************************************************************
            this.tratamientoJobScheduler.Start();
            this.produccionJobScheduler.Start();

            // Chequeamos el Servicio cada 4 horas.
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 14400000; // 60 minutos x 60 segundos x 1000 milisegundos x 4 horas
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            this.eventLogService.WriteEntry("Servicio CoplacaService iniciado correctamente");
        }

        //Otros eventos: OnPause, OnShutDown, OnContinue
        protected override void OnStop()
        {
            this.eventLogService.WriteEntry("Intentando detener el servicio CoplacaService....");

            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            //***************************************************************************
            //DETENEMOS LAS TAREAS EN EJECUCION
            //****************************************************************************
            this.tratamientoJobScheduler.Stop();
            this.produccionJobScheduler.Stop();

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            this.eventLogService.WriteEntry("Servicio CoplacaService detenido correctamente");
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            // Ejecutamos esta operación cada 4 horas para comprobar que todo sigue funcionando correctamente
            this.eventId++;

            Log.Information("Se ha ejecutado la comprobación rutinaria del estado del servicio...........................OK");

            this.eventLogService.WriteEntry("Monitorización del Sistema.", System.Diagnostics.EventLogEntryType.Information, this.eventId);
        }

        #endregion

        #region Funciones Privadas
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        private string CompruebaPreRequisitos()
        {
            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("urlAPIService"))
            {
                if (String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["urlAPIService"]))
                    return "No existe el valor para la clave <<urlAPIService>>...........................ERROR";
                else
                    Program.CurrentUrl = System.Configuration.ConfigurationManager.AppSettings["urlAPIService"];
            }
            else
                return "No existe la clave <<urlAPIService>> en el fichero de configuración...........................ERROR";

            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("userAPIService"))
            {
                if (String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["userAPIService"]))
                    return "No existe el valor para la clave <<userAPIService>>...........................ERROR";
                else
                    Program.CurrentUser = System.Configuration.ConfigurationManager.AppSettings["userAPIService"];
            }
            else
                return "No existe la clave <<userAPIService>> en el fichero de configuración...........................ERROR";

            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("passAPIService"))
            {
                if (String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["passAPIService"]))
                    return "No existe el valor para la clave <<passAPIService>>...........................ERROR";
                else
                    Program.CurrentPass = System.Configuration.ConfigurationManager.AppSettings["passAPIService"];
            }
            else
                return "No existe la clave <<passAPIService>> en el fichero de configuración...........................ERROR";

            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("coopAPIService"))
            {
                if (String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["coopAPIService"]))
                    return "No existe el valor para la clave <<coopAPIService>>...........................ERROR";
                else
                {
                    bool entero;
                    int cooperativa;
                    entero = int.TryParse(System.Configuration.ConfigurationManager.AppSettings["coopAPIService"], out cooperativa);

                    if (!entero)
                        return "El valor asignado a la clave <<coopAPIService>> no se corresponde con un número entero...........................ERROR";
                    else
                        Program.CurrentCoop = cooperativa;
                }
            }
            else
                return "No existe la clave <<coopAPIService>> en el fichero de configuración...........................ERROR";

            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("minutosTratamiento"))
            {
                if (String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["minutosTratamiento"]))
                    return "No existe el valor para la clave <<minutosTratamiento>>...........................ERROR";
                else
                {
                    bool entero;
                    int tratamientos;
                    entero = int.TryParse(System.Configuration.ConfigurationManager.AppSettings["minutosTratamiento"], out tratamientos);

                    if (!entero)
                        return "El valor asignado a la clave <<minutosTratamiento>> no se corresponde con un número entero...........................ERROR";
                    else
                        Program.minutosTratamiento = tratamientos;
                }
            }
            else
                return "No existe la clave <<minutosTratamiento>> en el fichero de configuración...........................ERROR";

            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("minutosProduccion"))
            {
                if (String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["minutosProduccion"]))
                    return "No existe el valor para la clave <<minutosProduccion>>...........................ERROR";
                else
                {
                    bool entero;
                    int produccion;
                    entero = int.TryParse(System.Configuration.ConfigurationManager.AppSettings["minutosTratamiento"], out produccion);

                    if (!entero)
                        return "El valor asignado a la clave <<minutosProduccion>> no se corresponde con un número entero...........................ERROR";
                    else
                        Program.minutosProduccion = produccion;
                }
            }
            else
                return "No existe la clave <<minutosProduccion>> en el fichero de configuración...........................ERROR";

            return String.Empty;
        }
        #endregion
    }
}
