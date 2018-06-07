using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Sistema_Bancario
{
    public class ContaCorrente
    {
        private int numDnd;
        private int numCta;
        private double saldo;
        public Cliente cliente;
        private List<ItemExtrato> listExtratoCta;

        public ContaCorrente(int numDnd,int numCta,Cliente cliente)
        {
            this.numDnd = numDnd;
            this.numCta = numCta;
            this.saldo = 0;
            this.cliente = cliente;
            listExtratoCta = new List<ItemExtrato>();
        }
        public ContaCorrente(int numDnd, int numCta,double saldo,Cliente cliente)
        {
            this.numDnd = numDnd;
            this.numCta = numCta;
            this.saldo = saldo;
            this.cliente = cliente;
            listExtratoCta = new List<ItemExtrato>();
        }
        public double Saldo { get => saldo; set => saldo = value; }
        public int NumDnd { get => numDnd; set => numDnd = value; }
        public int NumCta { get => numCta; set => numCta = value; }

        public bool Saque(double valorSaque)
        {
            if (valorSaque <= saldo && valorSaque>0)
            {
                this.saldo -= valorSaque;
                addTransacao("Saque", valorSaque);
                return true;
            }
            return false;
        }
        public bool Deposito(double valorDeposito)
        {
            if (valorDeposito > 0)
            {
                this.saldo += valorDeposito;
                addTransacao("Deposito", valorDeposito);
                return true;
            }
            return false;
           
        }
        public bool Transferencia(ContaCorrente contaCorrenteDestino, double valor)
        {
            if (valor > 2000)
            { throw new Exception("Valor de Transferência acima do limite. Limite máximo de transferência é de R$ 2000,00"); }

            if (valor > 0 && this.saldo >= valor)
            {
                this.saldo -= valor;
                contaCorrenteDestino.Deposito(valor);
                addTransacao("Transferencia", valor);
                return true;
            }

            return false;
        }
        public void addTransacao(string tipo,double valor)
        {
            listExtratoCta.Add(new ItemExtrato(tipo, valor));
        }
    }
}
