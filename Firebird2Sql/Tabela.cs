﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firebird2Sql
{
    public class Tabela
    {
        public Tabela()
        {
            DependenciasList = new List<string>();
        }
        public string Nome { get; set; }
        public List<string> DependenciasList { get; set; }
    }
}
