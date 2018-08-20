using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnviarCorreo
{
    public partial class auntentificacion : Form
    {
        public auntentificacion()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            visualizadorWeb browse = new visualizadorWeb();
            browse.ShowDialog();
            Dictionary<string, string> token = browse.auntentificacion;
            new enviarMensaje(token).ShowDialog();
            this.Close();
        }
    }
}
