using ECommerce.API.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace ECommerce.API.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private IDbConnection _connection;
        public UsuarioRepository()
        {
            //EAD server para não transportar o professor a lugares remotos, não
            //os alunos, pois este precisa proteger sua saúde.
            //Pool de Projetos.

            _connection = new SqlConnection(@"Data Source=.;Initial Catalog=ECommerce;Integrated Security=True;");
        }
        private static List<Usuario> _db = new List<Usuario>()
        {
        };

        public List<Usuario> Get()
        {

            return _connection.Query<Usuario>("select [Id], [Nome], [Email], [Sexo], [RG], [CPF], [NomeMae], [SituacaoCadastro], [DataCadastro] from usuarios (nolock)").ToList();
        }

        public Usuario Get(int id)
        {
            //até 7 objetos diferentes...
            return _connection.Query<Usuario,Contato, Usuario>(@"select 

                                                                 u.[Id], u.[Nome], u.[Email], u.[Sexo], u.[RG], u.[CPF],
                                                                 u.[NomeMae], u.[SituacaoCadastro], u.[DataCadastro] ,
                                                                 c.[Id], c.[UsuarioId], c.[Telefone], c.[Celular]

	                                                             from Usuarios u 
	                                                             left outer join Contatos c
	                                                             on u.Id = c.UsuarioId
	                                                             where u.Id = @Id

                                                               ", (usuario,contato) =>
                                                                {
                                                                    usuario.contato = contato;
                                                                    return usuario;

                                                                }

                                                                , new {Id = id}
                                                                  
                                                               
                                                               ).SingleOrDefault();
        }

        public void Insert(Usuario usuario)
        {
            _connection.Open();
            var transaction = _connection.BeginTransaction();

            try
            {
                #region Usuarios
                string sql = @"
                            INSERT INTO [dbo].[Usuarios]
                                       ([Nome]
                                       ,[Email]
                                       ,[Sexo]
                                       ,[RG]
                                       ,[CPF]
                                       ,[NomeMae]
                                       ,[SituacaoCadastro]
                                       ,[DataCadastro])
                                 VALUES
                                       (@Nome
                                       ,@Email
                                       ,@Sexo
                                       ,@RG
                                       ,@CPF
                                       ,@NomeMae
                                       ,@SituacaoCadastro
                                       ,@DataCadastro);
                            select  cast( scope_identity() as INT);
                            ";
                usuario.Id = _connection.Query<int>(sql, usuario, transaction).Single();
                #endregion

                if (usuario.contato == null)
                    return;


                usuario.contato.UsuarioId = usuario.Id;
                string sqlContato = @" INSERT INTO [dbo].[Contatos]  ([UsuarioId],[Telefone],[Celular]) VALUES (@UsuarioId, @Telefone,  @Celular);
                                   select  cast( scope_identity() as INT);";

                usuario.contato.Id = _connection.Query<int>(sqlContato, usuario.contato, transaction).Single();

                transaction.Commit();

            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (SystemException ex2) {
                    //Retornar para UsuarioController alguma mensagem
                    throw ex2;

                }
            }
            finally
            {
                _connection.Close();
            }



        }

        public void Update(Usuario usuario)
        {
            _connection.Open();
            var transaction = _connection.BeginTransaction();

            try
            {
                #region Usuarios
                string sql = @"
                           UPDATE [dbo].[Usuarios]
                           SET [Nome] = @Nome
                              ,[Email] = @Email
                              ,[Sexo] = @Sexo
                              ,[RG] = @RG
                              ,[CPF] = @CPF
                              ,[NomeMae] = @NomeMae
                              ,[SituacaoCadastro] = @SituacaoCadastro
                              ,[DataCadastro] = @DataCadastro
                            WHERE Id = @Id
                            ";

                _connection.Execute(sql, usuario, transaction);

                #endregion

                if (usuario.contato == null)
                    return;


                string sqlContato = @" 
                                    UPDATE [dbo].[Contatos]
                                       SET 
                                           [Telefone] = @Telefone
                                          ,[Celular] = @Celular
                                     WHERE id = @Id";

                _connection.Execute(sqlContato, usuario.contato, transaction);

                transaction.Commit();

            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (SystemException ex2)
                {
                    //Retornar para UsuarioController alguma mensagem
                    throw ex2;

                }
            }
            finally
            {
                _connection.Close();
            }



        }
        public void Delete(int id)
        {
            string sql = @"
                           Delete [dbo].[Usuarios]
                            WHERE Id = @Id
                            ";
            _connection.Execute(sql, new { Id = id });
        }

    }
}
