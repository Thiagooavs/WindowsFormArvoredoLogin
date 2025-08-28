using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsAppArvoredo.Models
{
    internal class Usuario
    {
        public int ID { get; set; }
        public string Login { get; set; }
        public string Nome { get; set; }
        public int NivelAcesso { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public int Ativo { get; set; }
        public DateTime DataCriacao { get; set; }

        public void Incluir()
        {
            try
            {
                Banco.AbrirConexao();

                string query = @"INSERT INTO usuarios (Login, Senha, Nome, Email, NivelAcesso, Ativo, DataCriacao) 
                               VALUES (@Login, @Senha, @Nome, @Email, @NivelAcesso, @Ativo, CURDATE())";

                Banco.Comando = new MySqlCommand(query, Banco.Conexao);

                // Usar parâmetros para evitar SQL Injection
                Banco.Comando.Parameters.AddWithValue("@Login", this.Login ?? "");
                Banco.Comando.Parameters.AddWithValue("@Senha", this.Senha ?? "");
                Banco.Comando.Parameters.AddWithValue("@Nome", this.Nome ?? "");
                Banco.Comando.Parameters.AddWithValue("@Email", this.Email ?? "");
                Banco.Comando.Parameters.AddWithValue("@NivelAcesso", this.NivelAcesso);
                Banco.Comando.Parameters.AddWithValue("@Ativo", this.Ativo);

                Banco.Comando.ExecuteNonQuery();
                Banco.FecharConexao();

                MessageBox.Show("Usuário incluído com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Erro ao incluir usuário: {e.Message}", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Banco.FecharConexao();
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

                string query = "SELECT ID, Nome, NivelAcesso FROM usuarios WHERE Login = @Login AND Senha = @Senha AND Ativo = 1";
                Banco.Comando = new MySqlCommand(query, Banco.Conexao);

                Banco.Comando.Parameters.AddWithValue("@Login", login);
                Banco.Comando.Parameters.AddWithValue("@Senha", senha);

                using (MySqlDataReader leitor = Banco.Comando.ExecuteReader())
                {
                    if (leitor.Read())
                    {
                        idUsuario = Convert.ToInt32(leitor["ID"]);
                        nomeUsuario = leitor["Nome"].ToString();
                        nivelAcesso = Convert.ToInt32(leitor["NivelAcesso"]);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao verificar credenciais: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Banco.FecharConexao();
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

        // Método para buscar todos os usuários (útil para administração)
        public static DataTable BuscarTodos()
        {
            DataTable dt = new DataTable();
            try
            {
                Banco.AbrirConexao();

                string query = "SELECT ID, Login, Nome, Email, NivelAcesso, Ativo, DataCriacao FROM usuarios ORDER BY Nome";
                Banco.Adaptador = new MySqlDataAdapter(query, Banco.Conexao);
                Banco.Adaptador.Fill(dt);

                Banco.FecharConexao();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Erro ao buscar usuários: {e.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Banco.FecharConexao();
            }
            return dt;
        }

        // Método para buscar usuário por ID
        public bool BuscarPorID(int id)
        {
            try
            {
                Banco.AbrirConexao();

                string query = "SELECT * FROM usuarios WHERE ID = @ID";
                Banco.Comando = new MySqlCommand(query, Banco.Conexao);
                Banco.Comando.Parameters.AddWithValue("@ID", id);

                using (MySqlDataReader leitor = Banco.Comando.ExecuteReader())
                {
                    if (leitor.Read())
                    {
                        this.ID = Convert.ToInt32(leitor["ID"]);
                        this.Login = leitor["Login"].ToString();
                        this.Nome = leitor["Nome"].ToString();
                        this.Email = leitor["Email"].ToString();
                        this.NivelAcesso = Convert.ToInt32(leitor["NivelAcesso"]);
                        this.Ativo = Convert.ToInt32(leitor["Ativo"]);
                        this.DataCriacao = Convert.ToDateTime(leitor["DataCriacao"]);

                        Banco.FecharConexao();
                        return true;
                    }
                }

                Banco.FecharConexao();
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Erro ao buscar usuário: {e.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Banco.FecharConexao();
                return false;
            }
        }
    }
}