using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnviarCorreo
{
    public partial class enviarMensaje : Form
    {
        Dictionary<string, string> token;
        private string codificado = string.Empty;
        public enviarMensaje(Dictionary<string, string> token)
        {
            InitializeComponent();
            this.token = token;
        }

        private void enviarMensaje_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPara.Text))
            {
                MessageBox.Show("Ingresar destinatario");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAsunto.Text))
            {
                MessageBox.Show("Ingresar asunto");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMensaje.Text))
            {
                MessageBox.Show("Ingresar el mensaje a enviar");
                return;
            }
            //Se agrega el mensaje con la norma RFC 2822
            /*
                From: John Doe <jdoe@machine.example>
                To: Mary Smith <mary@example.net>
                Subject: Saying Hello
                Date: Fri, 21 Nov 1997 09:55:06 -0600
                Message-ID: <1234@local.machine.example>

                This is a message just to say hello.
                So, "Hello".
            */

            

            string apiKey = "AIzaSyBIwHUDTKsz-XZab4LA-yjuGgxBQN9jVx0";
            string uri = string.Empty;


            WebClient cliente = new WebClient();
            cliente.Headers[HttpRequestHeader.Authorization] = $"{"Bearer".Trim()} {this.token["access_token"].Trim()}";
            //cliente.Headers[HttpRequestHeader.ContentType] = "application/json";
            if (string.IsNullOrWhiteSpace(txtImagen.Text))
            {
                uri = $"https://www.googleapis.com/gmail/v1/users/santiagoantoniomariscal%40gmail.com/messages/send?key={apiKey}";
                string formarMensaje = $"From: OFICINA DE PENSIONES <santiagoantoniomariscal@gmail.com>\n";
                formarMensaje += $"To: USUARIO <{txtPara.Text}>\n";
                formarMensaje += $"Subject: {txtAsunto.Text}\n";
                formarMensaje += $"Date: Fri, 21 Nov 1997 09:55:06 -0600\n";
                formarMensaje += $"Message-ID: <1234@local.machine.example>\n\n";
                formarMensaje += txtMensaje.Text;

                //Se convierte en base 64 para la norma del mensaje

                byte[] mensajeBytes = Encoding.UTF8.GetBytes(formarMensaje);
                string mensajeCodificado = Convert.ToBase64String(mensajeBytes);
                string mensajeRaw = mensajeCodificado.Replace("+", "-").Replace("/", "_");

                cliente.Headers[HttpRequestHeader.ContentType] = "application/json";
                string json = "{\"raw\":\"" + mensajeRaw + "\"}";
                cliente.UploadString(uri, json);
            }
            else {
                uri = "https://www.googleapis.com/upload/gmail/v1/users/santiagoantoniomariscal%40gmail.com/messages/send?uploadType=multipart";
                string formarMensaje = "From: PENSIONES <santiagoantoniomariscal@gmail.com>\n";//el correo from jamás debe cambiar
                formarMensaje += $"to: {txtPara.Text}\n";
                formarMensaje += $"Subject: {txtAsunto.Text}\n";
                formarMensaje += "MIME-Version: 1.0\n";
                formarMensaje += "Content-Type: multipart/mixed;\n";
                formarMensaje += "        boundary=\"limite1\"\n\n";
                formarMensaje += "En esta sección se prepara el mensaje\n\n";
                formarMensaje += "--limite1\n";
                formarMensaje += "Content-Type: text/plain\n";
                formarMensaje += $"{txtMensaje.Text}\n\n";
                formarMensaje += "--limite1\n";
                formarMensaje += "Content-Type: application/pdf;\n";
                formarMensaje += "Content-Transfer-Encoding: BASE64;\n\n";
                formarMensaje += this.codificado;
                System.Diagnostics.Debug.WriteLine(formarMensaje); //Se diagnostica como se enviara el archivo por medio del correo electrónico
                cliente.Headers[HttpRequestHeader.ContentType] = "message/rfc822";
                string cuerpoMensajeEnviar = formarMensaje;
                cliente.UploadString(uri, cuerpoMensajeEnviar);
            }
            MessageBox.Show("Mensaje enviado");        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult p = open.ShowDialog();
            if (p == DialogResult.OK) {
                string archivo = open.FileName;
                txtImagen.Text = archivo;
                byte[] archivobytes = File.ReadAllBytes(archivo);
                string codificado = Convert.ToBase64String(archivobytes);
                this.codificado = codificado;
            }
        }
    }
}
