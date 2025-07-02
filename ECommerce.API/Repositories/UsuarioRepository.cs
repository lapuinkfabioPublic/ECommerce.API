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
            List<Usuario> usuarios = new List<Usuario>();

            _connection.Query<Usuario, Contato, EnderecoEntrega, Departamento, Usuario>(
                                                                @" select  

                                                                    u.[Id], u.[Nome], u.[Email], u.[Sexo], u.[RG], u.[CPF],
                                                                    u.[NomeMae], u.[SituacaoCadastro], u.[DataCadastro] ,
                                                                    c.[Id], c.[UsuarioId], c.[Telefone], c.[Celular],
                                                                    e.[Id], e.[UsuarioId], e.[NomeEndereco], e.[CEP], 
                                                                    e.[Estado], e.[Cidade], e.[Bairro], 
                                                                    e.[Endereco], e.[Numero], e.[Complemento]
                                                                    ,d.[Id], d.[Nome]

                                                                    from Usuarios u 
                                                                    left outer join Contatos c
                                                                    on u.Id = c.UsuarioId
                                                                    left outer join EnderecosEntrega e
                                                                    on e.UsuarioId = u.Id
                                                                    left outer join UsuariosDepartamentos ud
                                                                    on ud.UsuarioId = u.id
                                                                    left outer join Departamentos d
                                                                    on d.Id = ud.DepartamentoId



                                                                    where u.Id = @Id

                                                               ", (usuario, contato, enderecosEntrega, departamento) =>
                                                                {
                                                                    var usuJaAdicionado = usuarios.SingleOrDefault(a => a.Id == usuario.Id);

                                                                    //Verificação do usuário
                                                                    if (usuJaAdicionado == null)
                                                                    {
                                                                        usuario.Enderecos = new List<EnderecoEntrega>();
                                                                        usuario.Departamentos = new List<Departamento>();
                                                                        usuario.contato = contato;
                                                                        usuarios.Add(usuario);
                                                                    }
                                                                    else
                                                                    {
                                                                        usuario = usuJaAdicionado;
                                                                    }

                                                                    //Verificação do endereço
                                                                    if(enderecosEntrega != null && usuario.Enderecos.SingleOrDefault(a=>a.Id==enderecosEntrega.Id) == null)
                                                                        usuario.Enderecos.Add(enderecosEntrega);

                                                                    //Verificação do departamento
                                                                    if (departamento!= null && usuario.Departamentos.SingleOrDefault(a => a.Id == departamento.Id) == null)
                                                                        usuario.Departamentos.Add(departamento);

                                                                    return usuario; //Precisar retornar qq coisa, pode até ser um NULL

                                                                }
                                                                 ,new { Id = id }
                                                               );
            return usuarios.SingleOrDefault();


            

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

                if (usuario.Enderecos != null && usuario.Enderecos.Count > 0)
                {
                    foreach (var enderecoEntrega in usuario.Enderecos)
                    {
                        string sqlEndereco = @"INSERT INTO [dbo].[EnderecosEntrega]
                                               ([UsuarioId]
                                               ,[NomeEndereco]
                                               ,[CEP]
                                               ,[Estado]
                                               ,[Cidade]
                                               ,[Bairro]
                                               ,[Endereco]
                                               ,[Numero]
                                               ,[Complemento])
                                         VALUES
                                               (@UsuarioId
                                               ,@NomeEndereco
                                               ,@CEP
                                               ,@Estado
                                               ,@Cidade
                                               ,@Bairro
                                               ,@Endereco
                                               ,@Numero
                                               ,@Complemento);select  cast( scope_identity() as INT)";
                        enderecoEntrega.UsuarioId = usuario.Id;
                        enderecoEntrega.Id = _connection.Query<int>(sqlEndereco, enderecoEntrega, transaction).Single();

                    }
                
                }

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


                if (usuario.Enderecos != null && usuario.Enderecos.Count > 0)
                {
                    string sqlDeletarEnderecos = "Delete from [dbo].[EnderecosEntrega] where UsuarioId = @Id";
                    _connection.Execute(sqlDeletarEnderecos, usuario, transaction);

                    if (usuario.Enderecos != null && usuario.Enderecos.Count > 0)
                    {
                        foreach (var enderecoEntrega in usuario.Enderecos)
                        {
                            string sqlEndereco = @"INSERT INTO [dbo].[EnderecosEntrega]
                                               ([UsuarioId]
                                               ,[NomeEndereco]
                                               ,[CEP]
                                               ,[Estado]
                                               ,[Cidade]
                                               ,[Bairro]
                                               ,[Endereco]
                                               ,[Numero]
                                               ,[Complemento])
                                         VALUES
                                               (@UsuarioId
                                               ,@NomeEndereco
                                               ,@CEP
                                               ,@Estado
                                               ,@Cidade
                                               ,@Bairro
                                               ,@Endereco
                                               ,@Numero
                                               ,@Complemento);select  cast( scope_identity() as INT)";
                            enderecoEntrega.UsuarioId = usuario.Id;
                            enderecoEntrega.Id = _connection.Query<int>(sqlEndereco, enderecoEntrega, transaction).Single();

                        }

                    }
                }

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
