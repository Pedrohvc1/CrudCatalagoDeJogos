using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApiCatalogoJogos.Exceptions;
using WebApiCatalogoJogos.InputModel;
using WebApiCatalogoJogos.Services;
using WebApiCatalogoJogos.ViewModel;

namespace WebApiCatalogoJogos.Controllers.V1
{
    [Route("api/V1[controller]")]
    [ApiController]
    public class JogosController : ControllerBase
    {
        //contrato(interface)
        private readonly IJogoService _jogoService; // readonly - pq n é de nossa responsabilidade dar uma instancia pra ela, pois o proprio aspNet vai fazer isso.


        public JogosController(IJogoService jogoService)   //construtor
        {
            _jogoService = jogoService;
        }

        /// <summary>
        /// Buscar todos os jogos de forma paginada
        /// </summary>
        /// <remarks>
        /// Não é possível retornar os jogos sem paginação
        /// </remarks>
        /// <param name="pagina">Indica qual página está sendo consultada. Mínimo 1</param>
        /// <param name="quantidade">Indica a quantidade de reistros por página. Mínimo 1 e máximo 50</param>
        /// <response code="200">Retorna a lista de jogos</response>
        /// <response code="204">Caso não haja jogos</response>   
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoViewModel>>> Obter([FromQuery, Range(1, int.MaxValue)] int pagina = 1,
            [FromQuery, Range(1, 50)] int quantidade = 5)
        {
            var jogos = await _jogoService.Obter(pagina, quantidade);

            if (jogos.Count() == 0)
                return NoContent();

            return Ok(jogos);
        }
        //public async Task<ActionResult<List<object>>> Obter() // por padrão utilizamos Task para garantir uma melhor performace quando falamos em requisição web
        //{
        //    var result = await _jogoService.Obter(1, 5); // o await é por conta do Task
        //    return Ok();
        //}

        /// <summary>
        /// Buscar um jogo pelo seu Id
        /// </summary>
        /// <param name="idJogo">Id do jogo buscado</param>
        /// <response code="200">Retorna o jogo filtrado</response>
        /// <response code="204">Caso não haja jogo com este id</response>

        [HttpGet("{idJogo:guid}")]
        public async Task<ActionResult<JogoViewModel>> Obter([FromRoute] Guid idJogo)
        {
            var jogo = await _jogoService.Obter(idJogo);

            if (jogo == null)
                return NoContent();

            return Ok(jogo);
        }
        //public async Task<ActionResult<List<JogoViewModel>>> Obter(Guid IdJogo) // GUID é uma struct que gera um valor unico e aleatório
        //{
        //    return Ok();
        //}

        /// <summary>
        /// Inserir um jogo no catálogo
        /// </summary>
        /// <param name="jogoInputModel">Dados do jogo a ser inserido</param>
        /// <response code="200">Cao o jogo seja inserido com sucesso</response>
        /// <response code="422">Caso já exista um jogo com mesmo nome para a mesma produtora</response>  

        [HttpPost]
        public async Task<ActionResult<JogoViewModel>> InserirJogo([FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                var jogo = await _jogoService.Inserir(jogoInputModel);

                return Ok(jogo);
            }
            catch (JogoJaCadastradoException ex)
            {
                return UnprocessableEntity("Já existe um jogo com este nome para esta produtora"); //stats 422, ja existe jogo
            }
        }
        //public async Task<ActionResult<JogoViewModel>> InserirJogo(JogoInputModel jogo) // recebe o jogo
        //{
        //    return Ok();
        //}

        /// <summary>
        /// Atualizar um jogo no catálogo
        /// </summary>
        /// /// <param name="idJogo">Id do jogo a ser atualizado</param>
        /// <param name="jogoInputModel">Novos dados para atualizar o jogo indicado</param>
        /// <response code="200">Cao o jogo seja atualizado com sucesso</response>
        /// <response code="404">Caso não exista um jogo com este Id</response>   


        [HttpPut("{idJogo:guid}")] // o Put atualiza tudo
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, jogoInputModel);

                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }
        //public async Task<ActionResult> AtualizarJogo(Guid idJogo, JogoInputModel jogo) // como ja sabe qual é o jogo que vai retornar, não vai precisar do object no ActionResult
        //{
        //    return Ok();
        //}

        /// <summary>
        /// Atualizar o preço de um jogo
        /// </summary>
        /// /// <param name="idJogo">Id do jogo a ser atualizado</param>
        /// <param name="preco">Novo preço do jogo</param>
        /// <response code="200">Cao o preço seja atualizado com sucesso</response>
        /// <response code="404">Caso não exista um jogo com este Id</response> 


        [HttpPatch("{idJogo:guid}/preco/{preco:double}")] // no Patch vai só atualizar uma parte expec. do recurso ex: preco
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromRoute] double preco)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, preco);

                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }
        //public async Task<ActionResult> AtualizarJogo(Guid idJogo, double preco)
        //{
        //    return Ok();
        //}

        /// <summary>
        /// Excluir um jogo
        /// </summary>
        /// /// <param name="idJogo">Id do jogo a ser excluído</param>
        /// <response code="200">Cao o preço seja atualizado com sucesso</response>
        /// <response code="404">Caso não exista um jogo com este Id</response>

        [HttpDelete("{idJogo:guid}")]
        public async Task<ActionResult> ApagarJogo([FromRoute] Guid idJogo)
        {
            try
            {
                await _jogoService.Remover(idJogo);

                return Ok();
            }
            catch (JogoNaoCadastradoException ex)           
            {
                return NotFound("Não existe este jogo"); //status 404, n existe esse jogo
            }
        }
        //public async Task<AcceptedResult> ApagarJogo(Guid idJogo)
        //{
        //    return Ok();
        //}

    }
}
