using System.Data.SqlTypes;

namespace APIDemo.models
{
    public class Transaccion
    {
        public int TransaccionId { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
        public int CuentaId { get; set; }
        public int? CuentaDestino { get; set; }
        public int? NumeroDepositoID { get; set; }
        public int CajeroID { get; set; }
        public decimal Monto { get; set; }

    }
}
