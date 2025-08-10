using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaEntidad;
using System.Data.SqlClient;
using System.Globalization;



namespace CapaDatos
{
    public class CD_Carrito
    {


        public bool ExisteCarrito(int idcliente, int idproducto)
        {
            bool resultado = true;
            

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.con))
                {
                    SqlCommand cmd = new SqlCommand("SP_ExisteCarrito", oconexion);
                    cmd.Parameters.AddWithValue("ID_Cliente", idcliente);
                    cmd.Parameters.AddWithValue("ID_Prod", idproducto);
                    cmd.Parameters.Add("Resultado", System.Data.SqlDbType.Bit).Direction = System.Data.ParameterDirection.Output;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);

                }
            }
            catch (Exception ex)
            {
                resultado = false;
               
            }
            return resultado;
        }



        public bool OperacionCarrito(int idcliente, int idproducto, bool sumar, out string Mensaje)
        {
            bool resultado = true;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.con))
                {
                    SqlCommand cmd = new SqlCommand("SP_OperacionCarrito", oconexion);
                    cmd.Parameters.AddWithValue("ID_Cliente", idcliente);
                    cmd.Parameters.AddWithValue("ID_Prod", idproducto);
                    cmd.Parameters.AddWithValue("Sumar", idcliente);
                    cmd.Parameters.Add("Resultado", System.Data.SqlDbType.Bit).Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", System.Data.SqlDbType.VarChar, 500).Direction = System.Data.ParameterDirection.Output;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }

       


        public int CantidadEnCarrito(int idcliente)
        {
            int resultado = 0;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.con))
                {
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM CARRITO WHERE ID_Cliente = @idcliente", oconexion);
                    cmd.Parameters.AddWithValue("@idcliente", idcliente);
                    cmd.CommandType = System.Data.CommandType.Text;
                    oconexion.Open();
                    resultado = Convert.ToInt32(cmd.ExecuteScalar());

                }
            }
            catch (Exception ex)
            {
                resultado = 0;


            }

            return resultado;
        }



        public List<Carrito> ListarProducto(int idcliente)
        {
            List<Carrito> lista = new List<Carrito>();

            try
            {
                using (SqlConnection oConexion = new(Conexion.con))
                {
                    string query = @"SELECT * FROM FN_ObtenerCarritoCliente(@idcliente)";


                    SqlCommand cmd = new SqlCommand(query, oConexion);
                    cmd.Parameters.AddWithValue("@idcliente", idcliente);
                    cmd.CommandType = System.Data.CommandType.Text;
                    oConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(
                                new Carrito()
                                {
                                    oProducto = new Producto() { 
                                        ID_Prod = Convert.ToInt32(dr["ID_Prod"]),
                                        Nombre = dr["Nombre"].ToString(),
                                        Precio = Convert.ToDecimal(dr["Precio"], new CultureInfo("es-HN")),
                                        RutaImagen = dr["RutaImagen"].ToString(),
                                        NombreImagen = dr["NombreImagen"].ToString(),
                                        oMarca = new Marca() { Descripcion = dr["DesMarca"].ToString()}
                                    },
                                    Cantidad = Convert.ToInt32(dr["Cantidad"]),
                                }

                            );

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                lista = new List<Carrito>();

            }

            return lista;

        }





        public bool EliminarCarrito(int idcliente, int idproducto)
        {
            bool resultado = true;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.con))
                {
                    SqlCommand cmd = new SqlCommand("SP_EliminarCarrito", oconexion);
                    cmd.Parameters.AddWithValue("ID_Cliente", idcliente);
                    cmd.Parameters.AddWithValue("ID_Prod", idproducto);
                    cmd.Parameters.Add("Resultado", System.Data.SqlDbType.Bit).Direction = System.Data.ParameterDirection.Output;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);

                }
            }
            catch (Exception ex)
            {
                resultado = false;

            }
            return resultado;
        }









    }
}
