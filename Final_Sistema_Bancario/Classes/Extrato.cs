using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Sistema_Bancario
{
    public class ItemExtrato
    {
        public string tipoTransacao;
        public string dataTrans;
        public double valor;
        public ItemExtrato(string tipoTransacao,double valor)
        {
            this.tipoTransacao = tipoTransacao;
            DateTime dt = DateTime.Now;
            this.dataTrans = dt.ToString("tt", CultureInfo.InvariantCulture);
            this.valor = valor;
        }
    }
}
