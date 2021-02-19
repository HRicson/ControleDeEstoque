using ControleDeEstoque.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ControleDeEstoque.Web.Models
{
    public class UsuarioModel
    {
        public static bool ValidarUsuario(string login, string senha)
        {   
            var retorno = false;
            using (var conexao = new SqlConnection())
            {
                //@"Password=abc123;Persist Security Info=True;User ID=sa;Initial Catalog=HeroApp;Data Source=SOLARIS\SQLEXPRESS")
                conexao.ConnectionString =
                    @"Data Source=SOLARIS\SQLEXPRESS; Initial Catalog=controle-estoque; User Id=admin; Password=123";
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "SELECT count(*) FROM usuario WHERE login = @login AND senha = @senha";

                    comando.Parameters.Add("@login", SqlDbType.VarChar).Value = login;
                    comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(senha);

                    retorno = (int)comando.ExecuteScalar() > 0;
                }
            }
            return retorno;
        }
    }
}