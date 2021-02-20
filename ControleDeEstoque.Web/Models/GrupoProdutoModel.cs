using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ControleDeEstoque.Web.Models
{
    public class GrupoProdutoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o nome.")]
        public string Nome { get; set; }

        [Required]
        public bool Ativo { get; set; }

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
                    comando.CommandText = "SELECT count(*) FROM grupo_produto ";

                    retorno = (int)comando.ExecuteScalar();
                }
            }
            return retorno;
        }

        public static List<GrupoProdutoModel> RecuperarLista(int pagina, int tamPagina)
        {
            List<GrupoProdutoModel> retorno = new List<GrupoProdutoModel>();

            using (SqlConnection conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (SqlCommand comando = new SqlCommand())
                {
                    var posicao = (pagina - 1) * tamPagina;

                    comando.Connection = conexao;
                    comando.CommandText = string.Format(
                        "SELECT * FROM grupo_produto ORDER BY nome offset {0} rows fetch next {1} rows only",
                        posicao > 0 ? posicao - 1 : 0, tamPagina);

                    SqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        retorno.Add(new GrupoProdutoModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }
                }
            }
            return retorno;
        }

        public static GrupoProdutoModel RecuperarPeloId(int id)
        {
            GrupoProdutoModel retorno = null;

            using (SqlConnection conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (SqlCommand comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "SELECT * FROM grupo_produto WHERE (id = @id)";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    SqlDataReader reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        retorno = new GrupoProdutoModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"]
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
                        comando.CommandText = "DELETE FROM grupo_produto WHERE (id = @id)";

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

            GrupoProdutoModel model = RecuperarPeloId(Id);

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
                        cmd.Append("INSERT INTO grupo_produto(nome, ativo)");
                        cmd.Append("VALUES (@nome, @ativo);");
                        cmd.Append("SELECT CONVERT(int, scope_identity())");

                        comando.CommandText = cmd.ToString();

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = Nome;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (Ativo ? 1 : 0);

                        retorno = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText = "UPDATE grupo_produto SET nome=@nome, ativo=@ativo WHERE id=@id";

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = Nome;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (Ativo ? 1 : 0);
                        comando.Parameters.Add("@id", SqlDbType.Int).Value = Id;

                        if (comando.ExecuteNonQuery() > 0)
                        {
                            retorno = Id;
                        }
                    }
                }
            }
            return retorno;
        }
    }
}