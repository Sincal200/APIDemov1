using APIDemo.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace APIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentaController : ControllerBase
    {
        public readonly string con;

        public CuentaController(IConfiguration configuration)
        {
            con = configuration.GetConnectionString("conexion");
        }

        [HttpGet]
        public IEnumerable<Cuenta> Get()
        {
            List<Cuenta> cuentas = new();

            using (SqlConnection connection = new(con))
            {
                connection.Open();
                using (SqlCommand cmd = new("sp_ObtenerCuentas", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            Cuenta cuenta = new Cuenta
                            {
                                CuentaId = reader.GetInt32(0),
                                Saldo = reader.GetDecimal(1)
                            };

                            cuentas.Add(cuenta);

                        }

                    }
                }
            }
            return cuentas;
        }

        [HttpPost]
        public void Post([FromBody] Cuenta c)
        {
            using (SqlConnection connection = new(con))
            {
                connection.Open();
                using (SqlCommand cmd = new("sp_CrearCuenta", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CuentaID", c.CuentaId);
                    cmd.Parameters.AddWithValue("@Saldo", c.Saldo);
                    cmd.ExecuteNonQuery();

                }
            }
        }
    }
}
