using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnviarCorreo
{
    public partial class visualizadorWeb : Form
    {
        private Dictionary<string, string> auntentificacion_;
        public Dictionary<string, string> auntentificacion { get {
                return auntentificacion_;
            } }
        public visualizadorWeb()
        {
            InitializeComponent();
            auntentificacion_ = new Dictionary<string, string>();
        }

        private void visualizadorWeb_Load(object sender, EventArgs e)
        {
            //Limpiando cookies del navegador
            //this.webBrowser1.Document.ExecCommand("ClearAuthenticationCache", false, null);

            //Armado de la url de envío de auntentificación...

            string[] scopes = {
                "https://mail.google.com/",
                "https://www.googleapis.com/auth/gmail.compose",
                "https://www.googleapis.com/auth/gmail.insert",
                "https://www.googleapis.com/auth/gmail.labels",
                "https://www.googleapis.com/auth/gmail.metadata",
                "https://www.googleapis.com/auth/gmail.modify",
                "https://www.googleapis.com/auth/gmail.readonly",
                "https://www.googleapis.com/auth/gmail.send",
                "https://www.googleapis.com/auth/gmail.settings.basic",
                "https://www.googleapis.com/auth/gmail.settings.sharing"
            };

            string uri = "https://accounts.google.com/o/oauth2/v2/auth?";
            string clienteId = "client_id=744279275328-2cq7sba2j0k3b4f85mpvv8fmoafsatqs.apps.googleusercontent.com";
            string responseType = "response_type=code";
            string scope = "scope=";
            string uri_redireccionamiento = "redirect_uri=http://localhost/redireccion";
            foreach (string item in scopes)
                scope += item + "%20";


            uri += $"{clienteId}&{responseType}&{scope}&{uri_redireccionamiento}";

            this.webBrowser1.Url = new Uri(uri);

            this.webBrowser1.Navigated += new WebBrowserNavigatedEventHandler(navego);


        }

        private void navego(object sender, WebBrowserNavigatedEventArgs e)
        {
            try
            {
                
                if (e.Url.AbsolutePath.Contains("/redireccion"))
                {
                    if (this.auntentificacion.Count != 0)
                        return;

                    //Se extrae el código para la autentificación Auth2.0 ;)
                    string uri = e.Url.AbsoluteUri;
                    int index = uri.IndexOf('?');
                    string fragmentoCodigo = uri.Substring(index + 1);
                    index = fragmentoCodigo.IndexOf('=');
                    string codigo = fragmentoCodigo.Substring(index + 1);
                    codigo = codigo.Substring(0, codigo.Length - 1);

                    //Intercambiamos el código por un identificador token...

                    string token = peticionToken(codigo);
                    token = token.Replace("{", "").Replace("}", "").Replace("\n", "").Replace("\"", "");
                    string[] valores = token.Split(',');
                    string tokenString = valores[0];
                    string tipoTokenString = valores[3];

                    string tokenSacado = tokenString.Split(':')[1];
                    string tipoTokenSacado = tipoTokenString.Split(':')[1];

                    auntentificacion_.Add("access_token", tokenSacado);
                    auntentificacion_.Add("token_type", tipoTokenSacado);
                    MessageBox.Show("Auntentificación realizada con exito!!","Aviso",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch(Exception er) {
                MessageBox.Show(er.Message);
            }
        }

        private string peticionToken(string codigoParametro)
        {
            string uri = "https://www.googleapis.com/oauth2/v4/token?";
            string codigo = $"code={codigoParametro}";
            string cliente_id = "client_id=744279275328-2cq7sba2j0k3b4f85mpvv8fmoafsatqs.apps.googleusercontent.com";
            string cliente_secreto = "client_secret=RSezg_UZGeMtTdZjbFKSArme";
            string uri_redirect = "redirect_uri=http://localhost/redireccion";
            string autorizacion = "grant_type=authorization_code";

            WebClient cliente = new WebClient();
            cliente.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            string parametros = $"{codigo}&{cliente_id}&{cliente_secreto}&{uri_redirect}&{autorizacion}";
            uri += parametros;
            var respuesta = cliente.UploadValues(uri,new NameValueCollection());
            string texto = Encoding.UTF8.GetString(respuesta);
            return texto;
        }

        
    }
}
