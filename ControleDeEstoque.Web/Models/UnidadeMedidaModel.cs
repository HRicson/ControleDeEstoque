using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ControleDeEstoque.Web.Models
{
    public class UnidadeMedidaModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o nome.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Preencha a sigla.")]
        public string Sigla { get; set; }

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
                    comando.CommandText = "SELECT count(*) FROM unidade_medida ";
                    retorno = (int)comando.ExecuteScalar();
                }
            }
            return retorno;
        }

        public static List<UnidadeMedidaModel> RecuperarLista(int pagina, int tamPagina)
        {
            List<UnidadeMedidaModel> retorno = new List<UnidadeMedidaModel>();

            using (SqlConnection conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (SqlCommand comando = new SqlCommand())
                {
                    var posicao = (pagina - 1) * tamPagina;

                    comando.Connection = conexao;
                    comando.CommandText = string.Format(
                        "SELECT * FROM unidade_medida ORDER BY nome offset {0} rows fetch next {1} rows only",
                        posicao > 0 ? posicao - 1 : 0, tamPagina);

                    SqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        retorno.Add(new UnidadeMedidaModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Sigla = (string)reader["sigla"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }
                }
            }
            return retorno;
        }

        public static UnidadeMedidaModel RecuperarPeloId(int id)
        {
            UnidadeMedidaModel retorno = null;

            using (SqlConnection conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (SqlCommand comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "SELECT * FROM unidade_medida WHERE (id = @id)";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    SqlDataReader reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        retorno = new UnidadeMedidaModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Sigla = (string)reader["sigla"],
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
                        comando.CommandText = "DELETE FROM unidade_medida WHERE (id = @id)";

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

            UnidadeMedidaModel model = RecuperarPeloId(Id);

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
                        cmd.Append("INSERT INTO unidade_medida(nome, sigla, ativo)");
                        cmd.Append("VALUES (@nome, @sigla, @ativo);");
                        cmd.Append("SELECT CONVERT(int, scope_identity())");

                        comando.CommandText = cmd.ToString();

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = Nome;
                        comando.Parameters.Add("@sigla", SqlDbType.VarChar).Value = Sigla;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (Ativo ? 1 : 0);

                        retorno = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText = "UPDATE unidade_medida SET nome=@nome, sigla=@sigla, ativo=@ativo WHERE id=@id";

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = Nome;
                        comando.Parameters.Add("@sigla", SqlDbType.VarChar).Value = Sigla;
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