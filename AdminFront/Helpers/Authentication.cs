using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MIA.Helpers
{
    public static class Authentication
    {
        /// <summary>
        /// Ruta de la aplicación
        /// </summary>
        private static readonly string ServerAuthentication = ConfigurationManager.AppSettings["ServerAuthentication"].ToString();

        /// <summary>
        /// Consume el endpoint /api/Login de la api que valida directorio activo.
        /// </summary>
        /// <param name="username">Nombre de usuario que realizará la autenticación.</param>
        /// <param name="password">Contraseña del usuario que realizará la autenticación.</param>
        /// <param name="error">Almacena la descrición para los casos que ocirre una exepción.</param>
        /// <returns>Booleano que indica si fue correcta la autenticación.</returns>
        public static bool ValidateLogin(string username, string password, out string error)
        {
            bool isValid = false;
            error = string.Empty;
            try
            {
                User user = new User()
                {
                    Username = username,
                    Password = password
                };

                // Parametrizar solicitud
                WebRequest requests = WebRequest.Create(ServerAuthentication + "/api/Login");
                requests.Method = "POST";
                string jsonData = JsonConvert.SerializeObject(user);
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonData);
                requests.ContentType = "application/Json";
                requests.ContentLength = byteArray.Length;
                System.IO.Stream dataStream = requests.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                // Ejecutar solicitud
                HttpWebResponse httpResponse = (HttpWebResponse)requests.GetResponse();
                using(System.IO.StreamReader response = new System.IO.StreamReader(httpResponse.GetResponseStream()))
                {
                    user = JsonConvert.DeserializeObject<User>(response.ReadToEnd());
                }
                httpResponse.Close();

                if (!string.IsNullOrEmpty(user.Error))
                {
                    error = user.Error;
                }
                else
                {
                    isValid = user.IsValid;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isValid;
        }

        private class User
        {
            public User()
            {
                Username = Password = SAMAccountName = Error = string.Empty;
                IsValid = false;
            }

            private string username;
            /// <summary>
            /// Nombre del usuario.
            /// </summary>
            public string Username
            {
                get { return username; }
                set { username = value; }
            }

            private string password;
            /// <summary>
            /// Contraseña del usuario.
            /// </summary>
            public string Password
            {
                get { return password; }
                set { password = value; }
            }

            private bool isValid;
            /// <summary>
            /// Indica si la autenticación del usuario es correcta o no.
            /// </summary>
            public bool IsValid
            {
                get { return isValid; }
                set { isValid = value; }
            }

            private string error;
            /// <summary>
            /// Variable que almacena descripción del error cuando se genera una excepción.
            /// </summary>
            public string Error
            {
                get { return error; }
                set { error = value; }
            }

            private string sAMAccountName;
            /// <summary>
            /// Nombre de usuario que será buscado en directorio activo.
            /// </summary>
            public string SAMAccountName
            {
                get { return sAMAccountName; }
                set { sAMAccountName = value; }
            }
        }
    }
}
