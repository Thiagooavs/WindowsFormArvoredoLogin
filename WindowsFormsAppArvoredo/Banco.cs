using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsAppArvoredo
{
    internal class Banco
    {

        //criando variaveis responsáveis pela conexão e consulta que serão usados em todo o projeto

        //Connection responsável pela conexão com o MySql
        public static MySqlConnection Conexao;

        //responsavel pelo comando das instruções a serem dadas
        public static MySqlCommand Comando;

        //DataTable responsável por ligar o banco em controles com a propriedade datasource
        public static MySqlDataAdapter Adaptador;

        //DataTable responsável por ligar o banco em controles com a propriedade datasource
        public static DataTable datTabela;



        // String de conexão para o banco hospedado na Clever Cloud
        private static string stringConexao = "Server=bczmqdz1mglh5lfrjoa3-mysql.services.clever-cloud.com;Port=3306;Database=bczmqdz1mglh5lfrjoa3;Uid=ukbos1icvubljiwu;Pwd=2v6L6QOGWXHA8RrKY5om;SslMode=Required;";

        public static void AbrirConexao()
        {
            try
            {
                Conexao = new MySqlConnection(stringConexao);
                Conexao.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Erro ao conectar com o banco de dados: {e.Message}", "Erro de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FecharConexao();
            }
        }

        public static void FecharConexao()
        {
            try
            {
                if (Conexao != null && Conexao.State == ConnectionState.Open)
                {
                    Conexao.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void CriarTabelaUsuarios()
        {
            try
            {
                AbrirConexao();

                // Criar tabela de usuários se não existir
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS usuarios (
                    ID INT AUTO_INCREMENT PRIMARY KEY,
                    Login VARCHAR(50) NOT NULL UNIQUE,
                    Senha VARCHAR(255) NOT NULL,
                    Nome VARCHAR(100) NOT NULL,
                    Email VARCHAR(100),
                    NivelAcesso INT DEFAULT 1,
                    Ativo TINYINT DEFAULT 1,
                    DataCriacao DATE NOT NULL
                )";

                Comando = new MySqlCommand(createTableQuery, Conexao);
                Comando.ExecuteNonQuery();

                // Verificar se já existe um usuário administrador
                Comando = new MySqlCommand("SELECT COUNT(*) FROM usuarios WHERE NivelAcesso = 3", Conexao);
                int adminCount = Convert.ToInt32(Comando.ExecuteScalar());

                // Se não existir um admin, criar um usuário padrão
                if (adminCount == 0)
                {
                    string insertAdminQuery = @"
                    INSERT INTO usuarios (Login, Senha, Nome, Email, NivelAcesso, Ativo, DataCriacao) 
                    VALUES ('admin', 'admin123', 'Administrador', 'admin@arvoredo.com', 3, 1, CURDATE())";

                    Comando = new MySqlCommand(insertAdminQuery, Conexao);
                    Comando.ExecuteNonQuery();

                    MessageBox.Show("Usuário administrador criado!\nLogin: admin\nSenha: admin123",
                                  "Primeiro Acesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                FecharConexao();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Erro ao criar estrutura do banco: {e.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FecharConexao();
            }
        }

        // Método para testar a conexão
        public static bool TestarConexao()
        {
            try
            {
                AbrirConexao();
                bool conectado = Conexao.State == ConnectionState.Open;
                FecharConexao();
                return conectado;
            }
            catch
            {
                return false;
            }
        }
    }

}

