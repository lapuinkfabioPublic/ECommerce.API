using ECommerce.API.Models;
using ECommerce.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private IUsuarioRepository _repository;
        public UsuariosController()
        {
            _repository = new UsuarioRepository();

        }
        [HttpGet]
        public IActionResult Get()
        { 
            return Ok(_repository.Get());//200 do HTTP
       
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id) {
            var usuario = _repository.Get(id);
            if (usuario != null)
                return Ok(_repository.Get(id));//200 do HTTP
            else
                return NotFound(); //Error HTTP 404
        }

        [HttpPost]
        public IActionResult Insert([FromBody] Usuario usuario) { 
            _repository.Insert(usuario);//200 do HTTP
            return Ok(usuario);
        }

        [HttpPut]
        public IActionResult Update([FromBody] Usuario usuario)
        {

            _repository.Update(usuario);//200 do HTTP
            return Ok(usuario);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _repository.Delete(id);//200 do HTTP
            return Ok();

        }




    }
}
