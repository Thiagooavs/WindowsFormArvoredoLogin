using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using WindowsFormsAppArvoredo.Models;

namespace WindowsFormsAppArvoredo
{
    public partial class FormLogin : Form
    {
        #region Desing e Layout

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
           int nLeft,
           int nTop,
           int nRight,
           int nBottom,
           int nWidthEllipse,
           int nHeightEllipse
        );
        
        public FormLogin()
        {
            InitializeComponent();

            // Registra o evento de pintura
            this.Paint += new PaintEventHandler(FormLogin_Paint);

            // Registra o evento de redimensionamento
            this.Resize += new EventHandler(FormLogin_Resize);

            // Aplica as bordas arredondadas
            this.FormBorderStyle = FormBorderStyle.None;
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            this.Text = "Login Arvoredo";
        }

        private void SetBackColorDegrade(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Rectangle gradient_rect = new Rectangle(0, 0, Width, Height);

            using (LinearGradientBrush br = new LinearGradientBrush(
                gradient_rect,
                Color.FromArgb(250, 230, 194),
                Color.FromArgb(180, 123, 57),
                90f))
            {
                ColorBlend colorBlend = new ColorBlend(3);
                colorBlend.Colors = new Color[]
                {
                   Color.FromArgb(250, 230, 194),
                   Color.FromArgb(198, 143, 86),
                   Color.FromArgb(180, 123, 57)
                };
                colorBlend.Positions = new float[] { 0f, 0.5f, 1f };
                br.InterpolationColors = colorBlend;
                graphics.FillRectangle(br, gradient_rect);
            }
        }

        private void FormLogin_Paint(object sender, PaintEventArgs e)
        {
            SetBackColorDegrade(sender, e);
        }

        private void FormLogin_Resize(object sender, EventArgs e)
        {

            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));


            this.Invalidate();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            btnEntrar.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnEntrar.Width, btnEntrar.Height, 100, 100));
            btnVoltar.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnVoltar.Width, btnVoltar.Height, 100, 100));
            Txt_Usuario.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Txt_Usuario.Width, Txt_Usuario.Height, 20, 20));
            Txt_Senha.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Txt_Senha.Width, Txt_Senha.Height, 20, 20));

            Txt_Senha.PasswordChar = '•';

            // Configurar placeholder nos campos de texto
            if (string.IsNullOrEmpty(Txt_Usuario.Text))
            {
                Txt_Usuario.Text = "Digite seu usuário";
                Txt_Usuario.ForeColor = Color.Gray;
            }


            Txt_Senha.KeyPress += (s, args) =>
            {
                if (args.KeyChar == (char)Keys.Enter) 
                { 
                    btnEntrar_Click(sender, e);
                    args.Handled = true;
                }
            };

            Txt_Usuario.KeyPress += (s, args) =>
            {
                if (args.KeyChar == (char)Keys.Enter)
                {
                    Txt_Senha.Focus();
                    args.Handled = true;
                }
            };

        }
        #endregion

        #region Eventos dos Botões

        //botão de voltar
        private void btnVoltar_Click(object sender, EventArgs e)
        {       
            this.Close();
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            string usuario = Txt_Usuario.Text.Trim();
            string senha = Txt_Senha.Text.Trim();

            // Limpar placeholder se existir
            if (usuario == "Digite seu usuário")
            {
                usuario = "";
            }

            // Verifica se os campos estão preenchidos
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(senha))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Mostrar cursor de espera
            this.Cursor = Cursors.WaitCursor;
            btnEntrar.Enabled = false;
            btnEntrar.Text = "Verificando...";

            try
            {
                // Verificar conexão com o banco
                if (!Banco.TestarConexao())
                {
                    MessageBox.Show("Não foi possível conectar ao banco de dados.\nVerifique sua conexão com a internet.",
                                  "Erro de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Usuario u = new Usuario();
               
                // Verifica as credenciais
                if (u.Verificar(usuario, senha))
                {
                   

                    //Oculta o formulário login
                    this.Hide();

                    // Cria e exibe a tela principal do sistema
                    TelaArvoredo telaArvoredo = new TelaArvoredo();
                    telaArvoredo.FormClosed += (s, args) => this.Close(); // Fecha o form de login quando a tela principal for fechada
                    telaArvoredo.Show();


                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Usuário ou senha incorretos.", "Erro de autenticação",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Txt_Senha.Clear();
                    Txt_Senha.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao realizar login: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Restaurar interface
                this.Cursor = Cursors.Default;
                btnEntrar.Enabled = true;
                btnEntrar.Text = "Login";
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // Fecha o aplicativo
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void pictureBoxMostrarSenha_Click(object sender, EventArgs e)
        {
            if (Txt_Senha.PasswordChar == '•')
            {
                // Mostra a senha
                Txt_Senha.PasswordChar = '\0';
                // Muda para o ícone de olho aberto
                ((PictureBox)sender).Image = Properties.Resources.senha_do_olho;
            }
            else
            {
                // Oculta a senha
                Txt_Senha.PasswordChar = '•';
                // Muda para o ícone de olho fechado
                ((PictureBox)sender).Image = Properties.Resources.senha_do_olho;
            }
        }

        #endregion
    }
}
