using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaEntidad;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace CapaDatos
{
    public class CD_Reporte
    {

        public Dashboard VerDashboard()
        {
            Dashboard objeto = new Dashboard();

            try
            {
                using (SqlConnection oConexion = new(Conexion.con))
                {
                    SqlCommand cmd = new SqlCommand("SP_ReporteDashboard", oConexion);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    oConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            objeto = new Dashboard()
                            {
                                TotalCliente = Convert.ToInt32(dr["TotalCliente"]),
                                TotalVenta = Convert.ToInt32(dr["TotalVenta"]),
                                TotalProducto = Convert.ToInt32(dr["TotalProducto"])
                            };

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                objeto = new Dashboard();

            }

            return objeto;

        }



        public List<Reporte> Ventas(string fechainicio, string fechafin, string idtransaccion)
        {
            List<Reporte> lista = new List<Reporte>();

            try
            {
                using (SqlConnection oConexion = new(Conexion.con))
                {
                    SqlCommand cmd = new SqlCommand("SP_ReporteVentas", oConexion);
                    cmd.Parameters.AddWithValue("fechainicio", fechainicio);
                    cmd.Parameters.AddWithValue("fechafin", fechafin);
                    cmd.Parameters.AddWithValue("idtransaccion", idtransaccion);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    oConexion.Open();

                     


                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(
                                new Reporte()
                                {
                                    FechaVenta = dr["FechaVenta"].ToString(),
                                    Cliente = dr["Cliente"].ToString(),
                                    Producto = dr["Producto"].ToString(),
                                    Precio = Convert.ToDecimal(dr["Precio"], new CultureInfo("hn-HN")) ,
                                    Cantidad = Convert.ToInt32(dr["Cantidad"]),
                                    Total = Convert.ToDecimal(dr["Total"], new CultureInfo("hn-HN")),
                                    ID_Transaccion = dr["ID_Transaccion"].ToString()

                                }

                            );

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                lista = new List<Reporte>();

            }

            return lista;

        }




    }
}
