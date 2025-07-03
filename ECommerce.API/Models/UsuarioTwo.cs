using Dapper.Contrib.Extensions;

namespace ECommerce.API.Models
{
    [Table("Usuarios")]
    public class UsuarioTwo
    {
        [Key]
        public int Cod { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }

        public string Sexo { get; set; }

        public string RG { get; set; }

        public string CPF { get; set; }

        public string NomeCompletoMae  { get; set; }

        public string SituacaoCadastro { get; set; }

        public DateTimeOffset DataCadastro { get; set; }

        [Write(false)]
        public Contato? contato { get; set; } /*1:1*/
        
        [Write(false)]
        public ICollection<EnderecoEntrega>? Enderecos { get; set; }/*1:N*/
        
        [Write(false)]
        public ICollection<Departamento>? Departamentos { get; set; }/*N:N*/


    }
}
