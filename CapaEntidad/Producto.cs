using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Producto
    {
        public int ID_Prod { get; set; }
        public string Nombre{ get; set; }
        public string Descripcion { get; set; }
        public Marca oMarca{ get; set; }
        public Categoria oCategoria{ get; set; }
        public decimal Precio { get; set; }
        public string PrecioTexto { get; set; }

        public int Stock { get; set; }
        public string RutaImagen { get; set; }
        public string NombreImagen { get; set; }
        public bool Activo { get; set; }


        //Base64 es un codigo para que se pueda ver la imagen en la web
        public string Base64 { get; set; }
        //La extension de la imagen .jpg, png, etc
        public string Extension { get; set; }

    }
}
