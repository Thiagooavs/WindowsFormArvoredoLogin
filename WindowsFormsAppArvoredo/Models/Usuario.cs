using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsAppArvoredo.Models
{
    internal class Usuario
    {
        public static int ID { get; set; }
        public static string Login { get; set; }
        public static string Nome { get; set; }
        public static int NivelAcesso { get; set; }
        public static string Email { get; set; }
        public static string Senha { get; set; }
        public static int Ativo { get; set; }


        public void Incluir()
        {
            try
            {
                Banco.AbrirConexao();

                Banco.Comando = new MySqlCommand("INSERT INTO usuarios(Login, Senha, Nome, Email, NivelAcesso, Ativo, DataCriacao) VALUES('thiagomassa', 'senha', 'thiago', 'Thiago@email', 1, 1, CURDATE())", Banco.Conexao);
                Banco.Comando.ExecuteNonQuery();

                Banco.FecharConexao();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        public bool VerificarCredenciais(string login, string senha, out int idUsuario, out string nomeUsuario, out int nivelAcesso)
        {
            idUsuario = 0;
            nomeUsuario = "";
            nivelAcesso = 0;


            try
            {
                Banco.AbrirConexao();
                Banco.Comando = new MySqlCommand("use arvoredo; SELECT ID, Nome, NivelAcesso FROM usuarios WHERE Login = @Login AND Senha = @Senha AND Ativo = 1 ", Banco.Conexao);

                Banco.Comando.Parameters.AddWithValue("@Login", Login);
                Banco.Comando.Parameters.AddWithValue("@Senha", Senha);

                Banco.Comando.ExecuteNonQuery();



                using (MySqlDataReader leitor = Banco.Comando.ExecuteReader())
                {
                    if (leitor.Read())
                    {

                        // Lê os dados do usuário
                        idUsuario = Convert.ToInt32(leitor["ID"]);
                        nomeUsuario = leitor["Nome"].ToString();
                        nivelAcesso = Convert.ToInt32(leitor["NivelAcesso"]);
                        Banco.FecharConexao();

                        return true;
                    }
                    return false;
                }

                
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao verificar credenciais: " + ex.Message);

            }
        }

        public bool Verificar(string usuario, string senha)
        {

            // Verifica as credenciais
            if (VerificarCredenciais(usuario, senha, out int idUsuario, out string nomeUsuario, out int nivelAcesso))
            {
                // Armazena informações do usuário logado
                UsuarioLogado.ID = idUsuario;
                UsuarioLogado.Login = usuario;
                UsuarioLogado.Nome = nomeUsuario;
                UsuarioLogado.NivelAcesso = nivelAcesso;

                // Exibe mensagem de boas-vindas
                MessageBox.Show($"Bem-vindo ao Sistema Arvoredo, {nomeUsuario}!",
                    "Login realizado com sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;

            }
            else
            {
                return false;
            }
        }
    }
}
