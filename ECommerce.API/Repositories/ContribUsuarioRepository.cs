using ECommerce.API.Models;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Dapper.Contrib.Extensions;

namespace ECommerce.API.Repositories
{
    public class ContribUsuarioRepository : IUsuarioRepository
    {
        private IDbConnection _connection;
        public ContribUsuarioRepository()
        {
            _connection = new SqlConnection(@"Data Source=.;Initial Catalog=ECommerce;Integrated Security=True;");
        }

        public List<Usuario> Get()
        {
            return _connection.GetAll<Usuario>().ToList();
        }

        public Usuario Get(int id)
        {
            return _connection.Get<Usuario>(id);
        }

        public void Insert(Usuario usuario)
        {
            usuario.Id = Convert.ToInt32(_connection.Insert(usuario));
        }

        public void Update(Usuario usuario)
        {
            _connection.Update(usuario);
        }

        public void Delete(int id)
        {
            _connection.Delete(Get(id));
        }

    }
}
