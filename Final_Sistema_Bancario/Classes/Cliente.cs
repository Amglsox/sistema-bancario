using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Sistema_Bancario
{
    public class Cliente
    {
        private string nome, cpf, dataNasc;
        public Cliente(string nome,string cpf,string dataNasc)
        {
            this.Nome = nome;
            this.Cpf = cpf;
            this.DataNasc = dataNasc;
        }

        public string Nome { get => nome; set => nome = value; }
        public string Cpf { get => cpf; set => cpf = value; }
        public string DataNasc { get => dataNasc; set => dataNasc = value; }
    }
}
