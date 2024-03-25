using APIDemo.models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace APIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowMyOrigin")]
    public class TranController : ControllerBase
    {
        public readonly string con;

        public TranController(IConfiguration configuration)
        {
            con = configuration.GetConnectionString("conexion");
        }

        [HttpGet]
        public IEnumerable<Transaccion> Get()
        {
            List<Transaccion> transaccions = new();

            using (SqlConnection connection = new(con))
            {
                connection.Open();
                using (SqlCommand cmd = new("sp_ObtenerTransacciones", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            Transaccion transaccion = new Transaccion
                            {
                                TransaccionId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                Fecha = reader.IsDBNull(1) ? DateTime.MaxValue : reader.GetDateTime(1),
                                Tipo = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                CuentaId = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                CuentaDestino = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                NumeroDepositoID = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                CajeroID = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                Monto = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7)
                            };

                            transaccions.Add(transaccion);

                        }

                    }
                }
            }
            return transaccions;
        }

        [HttpPost]
        public void Post([FromBody] Transaccion t)
        {
            using (SqlConnection connection = new(con))
            {
                connection.Open();
                using (SqlCommand cmd = new("sp_DepositoRetiro",connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransaccionID", t.TransaccionId);
                    cmd.Parameters.AddWithValue("@Fecha", t.Fecha);
                    cmd.Parameters.AddWithValue("@Tipo", t.Tipo);
                    cmd.Parameters.AddWithValue("@CuentaID", t.CuentaId);
                    if (t.CuentaDestino == null || t.CuentaDestino == 0)
                    {
                        cmd.Parameters.AddWithValue("@CuentaDestino", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@CuentaDestino", t.CuentaDestino);
                    }
                    if (t.NumeroDepositoID == null || t.NumeroDepositoID == 0)
                    {
                        cmd.Parameters.AddWithValue("@NumeroDepositoID", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@NumeroDepositoID", t.NumeroDepositoID);
                    }
                    cmd.Parameters.AddWithValue("@CajeroID", t.CajeroID);
                    cmd.Parameters.AddWithValue("@Monto", t.Monto);
                    cmd.ExecuteNonQuery();

                }
            }   
        }


    }
}
