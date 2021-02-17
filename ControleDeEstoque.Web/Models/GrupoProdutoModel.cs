using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;

namespace ControleDeEstoque.Web.Models
{
    public class GrupoProdutoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o nome.")]
        public string Nome { get; set; }

        [Required]
        public bool Ativo { get; set; }

        public static List<GrupoProdutoModel> RecuperarLista()
        {
            List<GrupoProdutoModel> retorno = new List<GrupoProdutoModel>();

            using (SqlConnection conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (SqlCommand comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "SELECT * FROM grupo_produto ORDER BY nome";
                    var reader = comando.ExecuteReader();
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
                    comando.CommandText = string.Format("SELECT * FROM grupo_produto WHERE (id = {0})", id);
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
                        comando.CommandText = string.Format("DELETE FROM grupo_produto WHERE (id = {0})", id);
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
                        comando.CommandText = string.Format("INSERT INTO grupo_produto(nome, ativo) VALUES ('{0}', {1});" +
                            "SELECT CONVERT(int, scope_identity())", Nome, Ativo ? 1 : 0);
                        retorno = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText = string.Format(
                            "UPDATE grupo_produto SET nome='{1}', ativo={2} WHERE id={0}",
                            Id, Nome, Ativo ? 1 : 0);

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