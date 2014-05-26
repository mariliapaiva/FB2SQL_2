using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firebird2Sql
{
    public class DependenciasNaoSatisfeitasException : Exception
    {
        const string formato = "Nem todas as dependências da tabela {0} foram selecionadas, tabelas não selecionadas:\n{1}";
        public DependenciasNaoSatisfeitasException(string tabela, IEnumerable<string> dependencias)
            : base(string.Format(formato, tabela, string.Join(", ", dependencias)))
        {
            this.Tabela = tabela;
            this.Dependencias = dependencias;
        }

        public string Tabela { get; set; }

        public IEnumerable<string> Dependencias { get; set; }
    }
}
