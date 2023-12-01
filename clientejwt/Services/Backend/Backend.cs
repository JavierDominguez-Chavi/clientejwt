using clientejwt.Models;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace clientejwt.Services.Backend
{
    public class Backend : IBackend
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public Backend(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        { 
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<List<UsuarioViewModel>> GetUsuariosAsync(string accessToken)
        {
            List<UsuarioViewModel> usuarios = new();

            // Preparo la llamada con el token
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPI"]}/home")
            {
                Headers = { { HeaderNames.Authorization, "Bearer " + accessToken } }
            };

            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                //Realizar llamada al Web API
                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    usuarios = await response.Content.ReadFromJsonAsync<List<UsuarioViewModel>>();
                }
            }
            catch (Exception)
            {
                // No se pudo conectar porque el servicio esta caido
            }
            return usuarios;
        }
        public async Task<AuthUser> AutenticationAsync(string correo, string password)
        {
            AuthUser token = null; // Variable para obtener la respuesta 
            LoginViewModel usuario = new()
            {
                Correo = correo,
                Password = password,
            };
            //Convertir a Json para enviar al usuario
            StringContent jsonContent = new(JsonSerializer.Serialize(usuario), Encoding.UTF8, "application/json");
            // Preparar la llamada con el JSON con los datos del login
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["UrlWebAPI"]}/login")
            {
                Content = jsonContent
            };

            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                //Realizar llamada al Web API
                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    token = await response.Content.ReadFromJsonAsync<AuthUser>();
                }
            }
            catch (Exception) 
            {
                // No se pudo conectar porque el servicio esta caido
            }

            return token;
        }
        public async Task<UsuarioViewModel> GetUsuarioAsync(string correo, string accessToken)
        { 
            UsuarioViewModel usuario = new();

            //Preparar llamada para el token

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPI"]}/home/{correo}")
            {
                Headers = { { HeaderNames.Authorization, "Bearer" + accessToken} }
            };
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                //Realizar llamada al Web API
                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    usuario = await response.Content.ReadFromJsonAsync<UsuarioViewModel>();
                }
            }
            catch (Exception)
            {
                // No se pudo conectar porque el servicio esta caido
            }
            return usuario;
        }

    }
}
