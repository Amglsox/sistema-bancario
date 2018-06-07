using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Final_Sistema_Bancario.Controller
{
    [RoutePrefix("api/Sistema_Bancario")]
    public class DefaultController : ApiController
    {
        public DefaultController()
        { }

        public HttpRequestMessage Request { get; set; }

        [HttpPost]
        [Route("CriarConta")]
        public bool PostCriaConta(Cliente cliente)
        {
            try
            {
                Conexao con = new Conexao();
                
                return con.CriarConta(cliente);
                //Request.CreateResponse(HttpStatusCode.OK,retorno);
            }
            catch (Exception ex)
            {

                return false;
                    //Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpGet]
        [Route("RealizaSaque/{cpf}/{valor}")]
        public bool RealizaSaque(string cpf,double valor)
        {
            try
            {
                Conexao con = new Conexao();
                ContaCorrente cc = con.SelectContaCorrente(cpf);
                if (cc.Saque(valor))
                {
                    if (con.updateSaldo(cc) && con.insertTransacao("Saque", cc, valor))
                        return true;
                    else
                        //return Request.CreateResponse(HttpStatusCode.OK, r);
                        return false;
                }
                else
                    return false;
                    //return Request.CreateResponse(HttpStatusCode.OK, "Saque não foi efetuado!");
            }
            catch (Exception ex)
            {
                return false;
                //return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpGet]
        [Route("RealizaDeposito/{cpf}/{valor}")]
        public bool RealizaDeposito(string cpf, double valor)
        {
            try
            {
                Conexao con = new Conexao();
                ContaCorrente cc = con.SelectContaCorrente(cpf);
                if (cc.Deposito(valor))
                {
                    if (con.updateSaldo(cc) && con.insertTransacao("Deposito", cc, valor))
                        return true;
                    else
                        //return Request.CreateResponse(HttpStatusCode.OK, r);
                        return false;
                }
                else
                    return false;
                    //return Request.CreateResponse(HttpStatusCode.OK, "Deposito não foi efetuado!");
            }
            catch (Exception ex)
            {
                return false;
                //return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("RealizaTransferencia/{cpfOrigem}/{cpfDestino}/{valor}")]
        public bool RealizaTransferencia(string cpfOrigem, string cpfDestino,double valor)
        {
            try
            {
                Conexao con = new Conexao();
                ContaCorrente ccOrigem = con.SelectContaCorrente(cpfOrigem);
                ContaCorrente ccDestino = con.SelectContaCorrente(cpfDestino);
                if (ccOrigem.Transferencia(ccDestino, valor))
                {
                    if (con.updateSaldo(ccOrigem) && con.updateSaldo(ccDestino) && con.insertTransacao("Transferencia", ccOrigem, valor) && con.insertTransacao("Transferencia", ccDestino, valor))
                        return true;
                    else
                        return false;
                    //return Request.CreateResponse(HttpStatusCode.OK, r);
                }
                else
                    return false;
                    //return Request.CreateResponse(HttpStatusCode.OK, "Deposito não foi efetuado!");
            }
            catch (Exception ex)
            {
                return false;
                //return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpGet]
        [Route("getTransacao/{numDnd}/{numCta}")]
        public int getTransacao(string numDnd,string numCta)
        {
            try
            {
                Conexao con = new Conexao();
                return con.getTransacao(numDnd, numCta);
            }
            catch (Exception ex)
            {
                return -2;
                //return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
    public class Conexao
    {
        private static string ConnectionString = "Data Source=DESKTOP-BUOJP8V;Initial Catalog=Sistema_Bancario;Integrated Security=True";
        public SqlConnection connection = new SqlConnection(ConnectionString);
        public SqlCommand AbreConexao()
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            return command;
        }
        public bool CriarConta(Cliente cli)
        {
            try
            {
                connection.Open();
                SqlCommand command = AbreConexao();
                command.CommandText = "INSERT INTO CLIENTE VALUES (@NOME,@CPF,@DATANASC)";
                command.Parameters.AddWithValue("NOME", cli.Nome);
                command.Parameters.AddWithValue("CPF", cli.Cpf);
                command.Parameters.AddWithValue("DATANASC", cli.DataNasc);
                command.ExecuteNonQuery();
                ContaCorrente cc = new ContaCorrente(1,MaxContaCorrente()+1,cli);
                command.CommandText = "INSERT INTO ContaCorrente VALUES (@numDnd,@numCta,@saldo,@idCliente)";
                command.Parameters.AddWithValue("numDnd",cc.NumDnd);
                command.Parameters.AddWithValue("numCta",cc.NumCta);
                command.Parameters.AddWithValue("saldo", cc.Saldo);
                command.Parameters.AddWithValue("idCliente", cc.cliente.Cpf);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
        }
        public int MaxContaCorrente()
        {
            SqlCommand command = AbreConexao();
            command.CommandText = "SELECT MAX(numCta) FROM ContaCorrente";
            try
            {
                string valor = command.ExecuteScalar().ToString();
                if (!String.IsNullOrEmpty(valor))
                {
                    return int.Parse(valor);
                }
                else
                    return 0;
            }
            catch 
            {
                return -1;
            }
            
            

        }
        public ContaCorrente SelectContaCorrente(string cpf)
        {
            try
            {
                connection.Open();
                SqlCommand command = AbreConexao();
                command.CommandText = "SELECT NUMDND,NUMCTA,SALDO,IDCLIENTE FROM CONTACORRENTE WHERE idCliente = @id";
                command.Parameters.AddWithValue("id", cpf);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();

                return new ContaCorrente(Convert.ToInt32(reader["NUMDND"]), Convert.ToInt32(reader["NUMCTA"]), Convert.ToDouble(reader["SALDO"]), SelectPessoa(cpf));
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                connection.Close();
            }
            
        }
        public Cliente SelectPessoa(string cpf)
        {
            try
            {
                connection.Close();
                connection.Open();
                SqlCommand command = AbreConexao();
                command.CommandText = "SELECT nome,cpf,dataNasc FROM Cliente WHERE cpf = @id";
                command.Parameters.AddWithValue("id", cpf);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return new Cliente(reader["nome"].ToString(), reader["cpf"].ToString(), reader["dataNasc"].ToString());
                
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public bool updateSaldo(ContaCorrente cc)
        {
            string retorno = "";
            try
            {
                connection.Open();
                SqlCommand command = AbreConexao();
                command.CommandText = "UPDATE ContaCorrente SET saldo = @valor WHERE idCliente = @id AND numDnd = @agencia AND numCta = @ctaCorrente";
                command.Parameters.AddWithValue("id", cc.cliente.Cpf);
                command.Parameters.AddWithValue("agencia", cc.NumDnd);
                command.Parameters.AddWithValue("ctaCorrente", cc.NumCta);
                command.Parameters.AddWithValue("valor", cc.Saldo);
                command.ExecuteNonQuery();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
        }
        public bool insertTransacao(string tipo,ContaCorrente cc,double valor)
        {
            try
            {
                connection.Open();
                SqlCommand command = AbreConexao();
                command.CommandText = "INSERT INTO Hist_Transacao VALUES(@tipoTrans,@dataTrans,@valor,@numDnd,@numCta)";
                command.Parameters.AddWithValue("tipoTrans", tipo);
                command.Parameters.AddWithValue("dataTrans", DateTime.Now);
                command.Parameters.AddWithValue("valor", valor);
                command.Parameters.AddWithValue("numDnd", cc.NumDnd);
                command.Parameters.AddWithValue("numCta", cc.NumCta);
                command.ExecuteNonQuery();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
        }
        public int getTransacao(string numDnd,string numCta)
        {
            int i = 0;
            try
            {
                connection.Open();
                SqlCommand command = AbreConexao();
                command.CommandText = "SELECT * FROM Hist_Transacao WHERE numDnd = @numDnd and numCta = @numCta";
                command.Parameters.AddWithValue("numDnd", int.Parse(numDnd));
                command.Parameters.AddWithValue("numCta", int.Parse(numCta));
                SqlDataReader reader = command.ExecuteReader();
                if(reader.HasRows)
                {
                    while (reader.Read())
                    {
                        i = i + 1;  
                    }

                }

                return i;

            }
            catch (Exception ex)
            {
                return -1;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
