using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WindowsGoiaService.APIService
{
    public class WebApiClient
    {
        #region Variables Privadas
        private HttpClient client;
        #endregion

        #region Propiedades públicas
        public System.Net.HttpStatusCode status { get; set; }
        #endregion

        public WebApiClient()
        {
            this.client = new HttpClient();
            this.client.DefaultRequestHeaders.Accept.Clear();
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public WebApiClient(string uri) : this()
        {
            this.client.BaseAddress = new Uri(uri);
        }
        public WebApiClient(string uri, string token): this(uri)
        {
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// FUNCION PARA ASIGNAR EL TOKEN A LAS LLAMADAS
        /// </summary>
        /// <param name="token">Cadena de texto con el token</param>
        /// <returns></returns>
        public void setToken(string token)
        {
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// FUNCION ASINCRONA PARA OBTENER EL TOKEN DE ACCESO Y PODER SEGUIR UTILIZANDO EL RESTO DE LA API
        /// </summary>
        /// <param name="email">Correo electrónico del usuario</param>
        /// <param name="pass">Contraseña asociada a ese correo</param>
        /// <returns></returns>
        public async Task<dynamic> Login(string email, string pass, int cooperativaid)
        {
            dynamic datos = null;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "token");
            request.Content = new StringContent("username=" + email + "&password=" + pass + "&cooperativaid=" + cooperativaid + "&grant_type=password", Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                datos = await response.Content.ReadAsAsync<dynamic>();
            }
            this.status = response.StatusCode;

            return datos;
        }

        /// <summary>
        /// FUNCION PARA COMPROBAR SI EL TOKEN ACTUAL ES VÁLIDO
        /// </summary>
        /// <param name="token">Cadena de texto con el token</param>
        /// <returns></returns>
        public async Task<dynamic> tryLoginWithToken(string token)
        {
            dynamic datos = null;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/account/getuser");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                datos = await response.Content.ReadAsAsync<dynamic>();
            }
            this.status = response.StatusCode;

            return datos;
        }

        /// <summary>
        /// FUNCION PARA EJECUTAR LLAMADAS GET A LA WEB API
        /// </summary>
        /// <param name="uri">Ruta relativa de acceso al servicio</param>
        /// <returns></returns>
        public async Task<dynamic> GetAsync(string uri)
        {
            dynamic datos = null;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                datos = await response.Content.ReadAsAsync<dynamic>();
            }
            this.status = response.StatusCode;

            return datos;
        }

        /// <summary>
        /// FUNCION PARA EJECUTAR LLAMADAS POST A LA WEB API
        /// </summary>
        /// <param name="uri">Ruta relativa de acceso al servicio</param>
        /// <param name="formBody">Contenido json que se desea enviar al servidor. 
        /// Por ejemplo, algo del tipo {\"name\":\"John Doe\",\"age\":33}</param>
        /// <returns></returns>
        public async Task<dynamic> PostAsync(string uri, string formBody)
        {
            dynamic datos = null;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(formBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                datos = await response.Content.ReadAsAsync<dynamic>();
            }
            this.status = response.StatusCode;

            return datos;
        }
    }
}
