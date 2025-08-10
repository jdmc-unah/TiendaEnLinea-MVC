using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaEntidad;

using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Ubicacion
    {
        public List<Departamento> ObtenerDepartamento()
        {
            List<Departamento> lista = new List<Departamento>();

            try
            {
                using (SqlConnection oConexion = new(Conexion.con))
                {
                    string query = "SELECT * FROM DEPARTAMENTO";
                    SqlCommand cmd = new SqlCommand(query, oConexion);

                    cmd.CommandType = System.Data.CommandType.Text;
                    oConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(
                                new Departamento()
                                {
                                    ID_Dep = dr["ID_Dep"].ToString(),
                                    Descripcion = dr["Descripcion"].ToString()
                                   

                                }
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lista = new List<Departamento>();

            }

            return lista;

        }




        public List<Municipio> ObtenerMunicipio(string iddepartamento)
        {
            List<Municipio> lista = new List<Municipio>();

            try
            {
                using (SqlConnection oConexion = new(Conexion.con))
                {
                    string query = "SELECT * FROM MUNICIPIO WHERE ID_Dep = @ID_Dep";
                    SqlCommand cmd = new SqlCommand(query, oConexion);
                    cmd.Parameters.AddWithValue("@ID_Dep", iddepartamento);
                     
                    cmd.CommandType = System.Data.CommandType.Text;
                    oConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(
                                new Municipio()
                                {
                                    ID_Mun = dr["ID_Mun"].ToString(),
                                    Descripcion = dr["Descripcion"].ToString()


                                }
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lista = new List<Municipio>();

            }

            return lista;

        }









    }
}
