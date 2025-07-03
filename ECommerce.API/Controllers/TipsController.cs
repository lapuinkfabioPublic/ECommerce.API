using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using ECommerce.API.Models;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipsController : ControllerBase
    {
        private IDbConnection _connection;
        public TipsController()
        {
            _connection = new SqlConnection(@"Data Source=.;Initial Catalog=ECommerce;Integrated Security=True;");
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id) 
        { 
            string sql = @"select [Id], [Nome], [Email], [Sexo], [RG], [CPF], [NomeMae], [SituacaoCadastro], [DataCadastro] from Usuarios where id=@Id; 

                         select [Id], [UsuarioId], [Telefone], [Celular] from Contatos where UsuarioId = @Id;

                        select  [Id], [UsuarioId], [NomeEndereco], [CEP], [Estado], [Cidade], [Bairro], [Endereco], [Numero], [Complemento] from EnderecosEntrega where UsuarioId = @Id;

                        select d.[Id], d.[Nome] from UsuariosDepartamentos ud inner join Departamentos d on ud.DepartamentoId = d.Id where ud.UsuarioId = @Id;";

            using (var multipleResultSet = _connection.QueryMultiple(sql, new { Id = id })) {

                var usuario = multipleResultSet.Read<Usuario>().SingleOrDefault();
                var contato = multipleResultSet.Read<Contato>().SingleOrDefault();
                var enderecos = multipleResultSet.Read<EnderecoEntrega>().ToList();
                var departamentos = multipleResultSet.Read<Departamento>().ToList();


                if (usuario != null) {

                    usuario.contato = contato;
                    usuario.Enderecos = enderecos;
                    usuario.Departamentos = departamentos;
                }
                return Ok(usuario);


            }
            return NotFound();
        }
    }
}
