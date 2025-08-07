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



        public static void AbrirConexao()
        {
            try
            {
                Conexao = new MySqlConnection("Server=localhost;port=3306;uid=root;pwd=");

                Conexao.Open();
            }
            catch(Exception e)
            {

                MessageBox.Show(e.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FecharConexao();

            }
        }

        public static void FecharConexao()
        {
            try
            {
                Conexao.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } 

        public static void CriarBanco()
        {
            try
            {

                AbrirConexao();

                Comando = new MySqlCommand("USE arvoredo", Conexao);

                Comando.ExecuteNonQuery();

                

                FecharConexao();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FecharConexao();

            }
        }
        
    }
}
