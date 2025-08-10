using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Cliente
    {


        public List<Cliente> Listar()
        {
            List<Cliente> lista = new List<Cliente>();

            try
            {
                using (SqlConnection oConexion = new(Conexion.con))
                {
                    string query = "SELECT ID_Cliente, Nombres, Apellidos , Correo, Clave, Reestablecer FROM Cliente";
                    SqlCommand cmd = new SqlCommand(query, oConexion);

                    cmd.CommandType = System.Data.CommandType.Text;
                    oConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(
                                new Cliente()
                                {
                                    ID_Cliente = Convert.ToInt32(dr["ID_Cliente"]),
                                    Nombres = dr["Nombres"].ToString(),
                                    Apellidos = dr["Apellidos"].ToString(),
                                    Correo = dr["Correo"].ToString(),
                                    Clave = dr["Clave"].ToString(),
                                    Reestablecer = Convert.ToBoolean(dr["Reestablecer"])

                                }

                            );

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                lista = new List<Cliente>();

            }

            return lista;

        }



        public int Registrar(Cliente obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.con))
                {
                    SqlCommand cmd = new SqlCommand("SP_RegistrarCliente", oconexion);
                    cmd.Parameters.AddWithValue("Nombres", obj.Nombres);
                    cmd.Parameters.AddWithValue("Apellidos", obj.Apellidos);
                    cmd.Parameters.AddWithValue("Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("Clave", obj.Clave);
                    cmd.Parameters.Add("Resultado", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", System.Data.SqlDbType.VarChar, 500).Direction = System.Data.ParameterDirection.Output;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    idautogenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                }
            }
            catch (Exception ex)
            {
                idautogenerado = 0;
                Mensaje = ex.Message;
            }
            return idautogenerado;
        }





        public bool CambiarClave(int cliente, string nuevaclave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.con))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE CLIENTE SET Clave = @nuevaclave , Reestablecer = 0 WHERE ID_Cliente = @idusuario", oconexion);
                    cmd.Parameters.AddWithValue("@idusuario", cliente);
                    cmd.Parameters.AddWithValue("@nuevaclave", nuevaclave);
                    cmd.CommandType = System.Data.CommandType.Text;
                    oconexion.Open();
                    resultado = cmd.ExecuteNonQuery() > 0 ? true : false;

                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;


            }

            return resultado;
        }



        public bool ReestablecerClave(int cliente, string clave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.con))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE CLIENTE SET Clave = @nuevaclave , Reestablecer = 1 WHERE ID_Cliente = @idusuario", oconexion);
                    cmd.Parameters.AddWithValue("@idusuario", cliente);
                    cmd.Parameters.AddWithValue("@nuevaclave", clave);
                    cmd.CommandType = System.Data.CommandType.Text;
                    oconexion.Open();
                    resultado = cmd.ExecuteNonQuery() > 0 ? true : false;

                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;


            }

            return resultado;
        }





    }
}
