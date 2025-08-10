using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Venta
    {
        public int ID_Venta { get; set; }
        public int ID_Cliente { get; set; }
        public int TotalProducto { get; set; }
        public decimal MontoTotal { get; set; }
        public string Telefono { get; set; }
        public string ID_Municipio { get; set; }
        public string Direccion { get; set; }
        public string ID_Transaccion { get; set; }
        public string Contacto { get; set; }

        public List<DetalleVenta> oDetVenta { get; set; }


    }
}
