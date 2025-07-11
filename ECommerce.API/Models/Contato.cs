﻿namespace ECommerce.API.Models
{
    public class Contato
    {
        public  int Id { get; set; }      
        public int UsuarioId { get; set; }

        public string Telefone { get; set; }
        
        public string Celular { get; set; }

        public DateTimeOffset DataCadastro { get; set; }

        public Usuario? Usuario { get; set; }

    }
}
