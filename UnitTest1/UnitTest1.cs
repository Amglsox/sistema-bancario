using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http;
using System.Net;
using Final_Sistema_Bancario;
using Final_Sistema_Bancario.Controller;
using System.Net.Http;

namespace UnitTest1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TesteCriarConta()
        {
            //ENVOLVE INSERÇÃO NO BANCO DE DADOS.
            var request = new DefaultController();
            request.Request = new HttpRequestMessage();
            var response = request.PostCriaConta(new Cliente("Rhayllander","13333333332","15/03/1997"));
            Assert.AreEqual(true, response);
            response = request.PostCriaConta(new Cliente("Claudiney", "13333333332", "15/03/1997"));
            Assert.AreEqual(false, response);
            response = request.PostCriaConta(new Cliente("Lucas Gama", "14444444444", "01/01/1990"));
            Assert.AreEqual(true, response);
            response = request.PostCriaConta(new Cliente(null, "12345", "01/01/1990"));
            Assert.AreEqual(false, response);
            response = request.PostCriaConta(new Cliente("Lucas Resende", "12222222222", "01/01/1990"));
            Assert.AreEqual(true, response);
            response = request.PostCriaConta(new Cliente("Alisson", "12222222222", "01/01/1990"));
            Assert.AreEqual(false, response);
        }
        [TestMethod]
        public void TestDeposito()
        {
            //ENVOLVE INSERÇÃO NO BANCO DE DADOS.
            var request = new DefaultController();
            request.Request = new HttpRequestMessage();
            Assert.AreEqual(true, request.RealizaDeposito("13333333332", 20));
            Assert.AreEqual(false, request.RealizaDeposito("13333333332", 0.00));

            Assert.AreEqual(true, request.RealizaDeposito("14444444444", 0.01));
            Assert.AreEqual(false, request.RealizaDeposito("14444444444", -0.001));

            Assert.AreEqual(true, request.RealizaDeposito("12222222222", 150));
            Assert.AreEqual(false, request.RealizaDeposito("12222222222", -150));
        }
        [TestMethod]
        public void TesteSaque()
        {
            var request = new DefaultController();         
            Assert.AreEqual(true, request.RealizaSaque("13333333332", 10));
            Assert.AreEqual(true, request.RealizaSaque("13333333332", 10));
            Assert.AreEqual(false, request.RealizaSaque("13333333332", 10));
            Assert.AreEqual(false, request.RealizaSaque("13333333332", -10));

            Assert.AreEqual(false, request.RealizaSaque("14444444444", 0));
            Assert.AreEqual(true, request.RealizaSaque("14444444444", 0.01));
            Assert.AreEqual(false, request.RealizaSaque("14444444444", 1));
            Assert.AreEqual(false, request.RealizaSaque("14444444444", -10));
            
            Assert.AreEqual(true, request.RealizaSaque("12222222222", 140));
            Assert.AreEqual(true, request.RealizaSaque("12222222222", 10));
            Assert.AreEqual(false, request.RealizaSaque("12222222222", 1));
            Assert.AreEqual(false, request.RealizaSaque("12222222222", -1));
        }
        [TestMethod]
        public void TestTransferencia()
        {
            var request = new DefaultController();
            request.RealizaDeposito("13333333332", 2000);
            request.RealizaDeposito("14444444444", 2000);
            request.RealizaDeposito("12222222222", 2000);
            //Testa modelo de negócio, não pode fazer transferência acima de 2000 reais.
            Assert.AreEqual(false, request.RealizaTransferencia("13333333332", "14444444444", 2001));
            Assert.AreEqual(true, request.RealizaTransferencia("13333333332", "14444444444", 2000));
            //Não realiza transferência de valor 0
            Assert.AreEqual(false, request.RealizaTransferencia("14444444444", "13333333332", 0));
            Assert.AreEqual(true, request.RealizaTransferencia("14444444444", "13333333332", 1000));
            //Não realiza transferência de valor negativo
            Assert.AreEqual(false, request.RealizaTransferencia("14444444444", "12222222222", -100));
            Assert.AreEqual(true, request.RealizaTransferencia("14444444444", "12222222222", 1001));
            Assert.AreEqual(true, request.RealizaTransferencia("14444444444", "12222222222", 1000));

        }

        [TestMethod]
        public void TestGetExtrato()
        {
            var request = new DefaultController();
            Assert.AreEqual(6, request.getTransacao("1","1"));
            Assert.AreNotEqual(0, request.getTransacao("1", "1"));
            Assert.AreEqual(-1, request.getTransacao("1", null));
        }
        [TestMethod]
        public void TesteSaqueClasse()
        {
            Cliente cli = new Cliente("Lucas", "13793622614", "15/03/2018");
            ContaCorrente cc = new ContaCorrente(0001, 101564659, cli);
            cc.Deposito(1000);
            Assert.AreEqual(true, cc.Saque(500));
            Assert.AreEqual(true, cc.Saque(450));
            Assert.AreEqual(true, cc.Saque(50));
            Assert.AreEqual(false, cc.Saque(1));
            Assert.AreEqual(false, cc.Saque(0));
        }
        [TestMethod]
        public void TestDepositoClasse()
        {
            Cliente cliente1 = new Cliente("Teste1", "94658711254", "06/11/1997");
            ContaCorrente conta1 = new ContaCorrente(0954, 154978777, cliente1);

            Assert.AreEqual(false, conta1.Deposito(0.00));
            Assert.AreEqual(true, conta1.Deposito(0.01));
            Assert.AreEqual(false, conta1.Deposito(-0.01));
            Assert.AreEqual(true, conta1.Deposito(150.00));
            Assert.AreEqual(false, conta1.Deposito(-150.00));
            Assert.AreEqual(true, conta1.Deposito(double.MaxValue));
            Assert.AreEqual(false, conta1.Deposito(double.MinValue));
        }
        [TestMethod]
        public void TestClienteClasse()
        {
            Cliente cliente1 = new Cliente("Teste1", "94658711254", "06/11/1997");

            Assert.AreEqual("Teste1", cliente1.Nome);
            Assert.AreEqual("94658711254", cliente1.Cpf);
            Assert.AreEqual("06/11/1997", cliente1.DataNasc);

            Cliente cliente2 = new Cliente("Teste2", "13264587988", "01/01/1990");

            Assert.AreEqual("Teste2", cliente2.Nome);
            Assert.AreEqual("13264587988", cliente2.Cpf);
            Assert.AreEqual("01/01/1990", cliente2.DataNasc);
        }
        [TestMethod]
        public void TestTransferenciaClasse()
        {
            Cliente cliente = new Cliente("Test", "123.456.789-01", "01/01/1990");

            ContaCorrente cc1 = new ContaCorrente(1234, 12345678, cliente);
            cc1.Deposito(4000);
            Assert.AreEqual(4000, cc1.Saldo);

            ContaCorrente cc2 = new ContaCorrente(1234, 12345678, cliente);
            cc2.Deposito(4000);
            Assert.AreEqual(4000, cc2.Saldo);

            bool t1 = cc1.Transferencia(cc2, 100);
            Assert.IsTrue(t1);
            Assert.AreEqual(3900, cc1.Saldo);
            Assert.AreEqual(4100, cc2.Saldo);

            bool t2 = cc1.Transferencia(cc2, 2000);
            Assert.IsTrue(t2);
            Assert.AreEqual(1900, cc1.Saldo);
            Assert.AreEqual(6100, cc2.Saldo);

            Assert.ThrowsException<Exception>(() => cc2.Transferencia(cc1, 2000.01), "Valor de Transferência acima do limite. Limite máximo de transferência é de R$ 2000,00");
            Assert.AreEqual(1900, cc1.Saldo);
            Assert.AreEqual(6100, cc2.Saldo);

            bool t3 = cc1.Transferencia(cc2, 0.01);
            Assert.IsTrue(t2);
            Assert.AreEqual(1899.99, cc1.Saldo);
            Assert.AreEqual(6100.01, cc2.Saldo);

            bool t4 = cc2.Transferencia(cc1, 0);
            Assert.IsFalse(t4);
            Assert.AreEqual(1899.99, cc1.Saldo);
            Assert.AreEqual(6100.01, cc2.Saldo);

            bool t5 = cc1.Transferencia(cc2, -100);
            Assert.IsFalse(t5);
            Assert.AreEqual(1899.99, cc1.Saldo);
            Assert.AreEqual(6100.01, cc2.Saldo);
        }

    }
}
