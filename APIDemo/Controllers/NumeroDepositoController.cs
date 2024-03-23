using APIDemo.models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace APIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NumeroDepositoController
    {
        public readonly string con;

        public NumeroDepositoController(IConfiguration configuration)
        {
            con = configuration.GetConnectionString("conexion");
        }

        [HttpGet]
        public IEnumerable<NumeroDeposito> Get()
        {
            List<NumeroDeposito> numeroDepositos = new();

            using (SqlConnection connection = new(con))
            {
                connection.Open();
                using (SqlCommand cmd = new("sp_ObtenerNumerosDepositos", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            NumeroDeposito numeroDeposito = new NumeroDeposito
                            {
                                NumeroDepositoID = reader.GetInt32(0),
                                Saldo = reader.GetDecimal(1)
                            };

                            numeroDepositos.Add(numeroDeposito);

                        }

                    }
                }
            }
            return numeroDepositos;
        }

        [HttpPost]
        public void Post([FromBody] NumeroDeposito n)
        {
            using (SqlConnection connection = new(con))
            {
                connection.Open();
                using (SqlCommand cmd = new("sp_CrearNumeroDeposito", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NumeroDepositoID", n.NumeroDepositoID);
                    cmd.Parameters.AddWithValue("@Saldo", n.Saldo);
                    cmd.ExecuteNonQuery();

                }
            }
        }
    }
}
