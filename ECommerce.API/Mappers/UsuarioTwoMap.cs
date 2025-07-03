using Dapper.FluentMap.Mapping;
using ECommerce.API.Models;

namespace ECommerce.API.Mappers
{
    public class UsuarioTwoMap : EntityMap<UsuarioTwo>
    {
        public UsuarioTwoMap()
        {
            Map(p => p.Cod).ToColumn("Id");
            Map(p => p.NomeCompleto).ToColumn("Nome");
            Map(p => p.NomeCompletoMae).ToColumn("NomeMae");
        }
    }
}
