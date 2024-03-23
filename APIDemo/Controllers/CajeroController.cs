using APIDemo.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace APIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CajeroController : ControllerBase
    {
        public readonly string con;

        public CajeroController(IConfiguration configuration)
        {
            con = configuration.GetConnectionString("conexion");
        }

        [HttpGet]
        public IEnumerable<Cajero> Get()
        {
            List<Cajero> cajeros = new();

            using (SqlConnection connection = new(con))
            {
                connection.Open();
                using (SqlCommand cmd = new("sp_ObtenerCajeros", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            Cajero cajero = new Cajero
                            {
                                CajeroID = reader.GetInt32(0),
                                Biletes100 = reader.GetInt32(1),
                                Billetes50 = reader.GetInt32(2),
                            };

                            cajeros.Add(cajero);

                        }

                    }
                }
            }
            return cajeros;
        }

        [HttpPost]
        public void Post([FromBody] Cajero c)
        {
            using (SqlConnection connection = new(con))
            {
                connection.Open();
                using (SqlCommand cmd = new("sp_CrearCajero", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CajeroID", c.CajeroID);
                    cmd.Parameters.AddWithValue("@Biletes100", c.Biletes100);
                    cmd.Parameters.AddWithValue("@Billetes50", c.Billetes50);
                    cmd.ExecuteNonQuery();

                }
            }
        }
    }
}
