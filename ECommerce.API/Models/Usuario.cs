using Dapper.Contrib.Extensions;

namespace ECommerce.API.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        public string Sexo { get; set; }

        public string RG { get; set; }

        public string CPF { get; set; }

        public string NomeMae  { get; set; }

        public string SituacaoCadastro { get; set; }

        public DateTimeOffset DataCadastro { get; set; }

        public Contato? contato { get; set; } /*1:1*/
                            
        public ICollection<EnderecoEntrega>? Enderecos { get; set; }/*1:N*/

        public ICollection<Departamento>? Departamentos { get; set; }/*N:N*/


    }
}
