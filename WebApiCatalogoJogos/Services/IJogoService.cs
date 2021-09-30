using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCatalogoJogos.InputModel;
using WebApiCatalogoJogos.ViewModel;

namespace WebApiCatalogoJogos.Services
{
    public interface IJogoService : IDisposable //  IDisposable oferece a capacidade de destruir o objeto
    {
        Task<List<JogoViewModel>> Obter(int pagina, int quantidade); //paginacao
        Task<JogoViewModel> Obter(Guid id);
        Task<JogoViewModel> Inserir(JogoInputModel jogo);
        Task Atualizar(Guid id, JogoInputModel jogo);
        Task Atualizar(Guid id, double preco);
        Task Remover(Guid id);
    }
}
