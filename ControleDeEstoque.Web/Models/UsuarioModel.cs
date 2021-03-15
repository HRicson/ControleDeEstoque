using ControleDeEstoque.Web.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ControleDeEstoque.Web.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o login")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Informe a senha")]
        public string Senha { get; set; }
        [Required(ErrorMessage = "Informe o nome")]
        public string Nome { get; set; }

        public static UsuarioModel ValidarUsuario(string login, string senha)
        {
            UsuarioModel retorno = null;

            using (var conexao = new SqlConnection())
            {
                //@"Password=abc123;Persist Security Info=True;User ID=sa;Initial Catalog=HeroApp;Data Source=SOLARIS\SQLEXPRESS")
                conexao.ConnectionString =
                    @"Data Source=SOLARIS\SQLEXPRESS; Initial Catalog=controle-estoque; User Id=admin; Password=123";
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "SELECT * FROM usuario WHERE login = @login AND senha = @senha";

                    comando.Parameters.Add("@login", SqlDbType.VarChar).Value = login;
                    comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(senha);

                    SqlDataReader reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        retorno = new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Login = (string)reader["login"],
                            Senha = (string)reader["senha"],
                            Nome = (string)reader["Nome"]
                        };
                    }
                }
            }
            return retorno;
        }

        public static List<UsuarioModel> RecuperarLista(int pagina = -1, int tamPagina = -1)
        {
            List<UsuarioModel> retorno = new List<UsuarioModel>();

            using (SqlConnection conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (SqlCommand comando = new SqlCommand())
                {
                    var posicao = (pagina - 1) * tamPagina;

                    comando.Connection = conexao;

                    if (pagina == -1 || tamPagina == -1)
                    {
                        comando.CommandText = "SELECT * FROM usuario ORDER BY nome";
                    }
                    else
                    {
                        comando.CommandText = string.Format(
                        "SELECT * FROM usuario ORDER BY nome offset {0} rows fetch next {1} rows only",
                        posicao > 0 ? posicao - 1 : 0, tamPagina);
                    }

                    SqlDataReader reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        retorno.Add(new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"]
                        });
                    }
                }
            }
            return retorno;
        }
        public static int RecuperarQuantidade()
        {
            int retorno = 0;

            using (SqlConnection conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (SqlCommand comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "SELECT count(*) FROM usuario";

                    retorno = (int)comando.ExecuteScalar();
                }
            }
            return retorno;
        }

        public static UsuarioModel RecuperarPeloId(int id)
        {
            UsuarioModel retorno = null;

            using (SqlConnection conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (SqlCommand comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "SELECT * FROM usuario WHERE (id = @id)";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    SqlDataReader reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        retorno = new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"]
                        };
                    }
                }
            }
            return retorno;
        }

        public static bool ExcluirPeloId(int id)
        {
            bool retorno = false;

            if (RecuperarPeloId(id) != null)
            {
                using (SqlConnection conexao = new SqlConnection())
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();

                    using (SqlCommand comando = new SqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = "DELETE FROM usuario WHERE (id = @id)";

                        comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                        retorno = (comando.ExecuteNonQuery() > 0);
                    }
                }
            }
            return retorno;
        }

        public int Salvar()
        {
            int retorno = 0;

            UsuarioModel model = RecuperarPeloId(Id);

            using (SqlConnection conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (SqlCommand comando = new SqlCommand())
                {
                    comando.Connection = conexao;

                    if (model == null)
                    {
                        StringBuilder cmd = new StringBuilder();
                        cmd.Append("INSERT INTO usuario(nome, login, senha)");
                        cmd.Append("VALUES (@nome, @login, @senha);");
                        cmd.Append("SELECT CONVERT(int, scope_identity())");

                        comando.CommandText = cmd.ToString();

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = Nome;
                        comando.Parameters.Add("@login", SqlDbType.VarChar).Value = Login;
                        comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(Senha);

                        retorno = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        StringBuilder cmd = new StringBuilder();
                        cmd.Append("UPDATE usuario SET ");
                        cmd.Append("nome = @nome, ");
                        cmd.Append("login = @login ");
                        if (!string.IsNullOrEmpty(Senha)) cmd.Append(", senha=@senha ");
                        cmd.Append("WHERE id = @id");

                        comando.CommandText = cmd.ToString();

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = Nome;
                        comando.Parameters.Add("@login", SqlDbType.VarChar).Value = Login;
                        comando.Parameters.Add("@id", SqlDbType.Int).Value = Id;

                        if (!string.IsNullOrEmpty(Senha))
                        {
                            comando.Parameters.Add("@senha", SqlDbType.VarChar)
                                .Value = CriptoHelper.HashMD5(Senha);
                        }

                        if (comando.ExecuteNonQuery() > 0)
                        {
                            retorno = Id;
                        }
                    }
                }
            }
            return retorno;
        }

        public string RecuperarStringNomePerfis()
        {
            string retorno = string.Empty;

            using (SqlConnection conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (SqlCommand comando = new SqlCommand())
                {
                    comando.Connection = conexao;

                    StringBuilder cmd = new StringBuilder();
                    cmd.Append("SELECT p.nome ");
                    cmd.Append("FROM perfil_usuario pu, perfil p ");
                    cmd.Append("WHERE (pu.id_usuario = @id_usuario) and (pu.id_perfil = p.id) and (p.ativo = 1) ");

                    comando.CommandText = cmd.ToString();
                    comando.Parameters.Add("@id_usuario", SqlDbType.Int).Value = Id;


                    SqlDataReader reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        retorno += (retorno != string.Empty ? ";" : string.Empty) + (string)reader["nome"];
                    }
                }
            }
            return retorno;
        }
    }
}