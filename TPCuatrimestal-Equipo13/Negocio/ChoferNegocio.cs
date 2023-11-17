﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;


namespace Negocio
{
    public class ChoferNegocio
    {
        private List<Chofer> listaChoferes = new List<Chofer>();

        //OBTIENE TODAS LAS ZONAS
        public List<Zona> ObtenerZonas(int idZona = -1)
        {
            AccesoDatos datos = new AccesoDatos();
            List<Zona> listAux = new List<Zona>();

            try
            {
                if (idZona == -1)
                {
                    datos.SetearConsulta("SELECT IDZONA, NOMBREZONA FROM ZONAS");

                    datos.EjecutarConsulta();

                    while (datos.Lector.Read())
                    {
                        Zona aux = new Zona();

                        aux.IDZona = datos.Lector["IDZONA"] is DBNull ? -1 : (int)datos.Lector["IDZONA"];
                        aux.NombreZona = datos.Lector["NOMBREZONA"] is DBNull ? "S/Z" : (string)datos.Lector["NOMBREZONA"];

                        listAux.Add(aux);
                    }

                }
                else
                {
                    datos.SetearConsulta("SELECT IDZONA, NOMBREZONA FROM ZONAS WHERE IDZONA = @IDZONA");
                    datos.SetearParametro("@IDZONA", idZona);
                    datos.EjecutarConsulta();
                    datos.Lector.Read();

                    Zona aux = new Zona();

                    aux.IDZona = datos.Lector["IDZONA"] is DBNull ? -1 : (int)datos.Lector["IDZONA"];
                    aux.NombreZona = datos.Lector["NOMBREZONA"] is DBNull ? "S/Z" : (string)datos.Lector["NOMBREZONA"];

                    listAux.Add(aux);
                }

                return listAux;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public List<Chofer> ObtenerDatos(int idChofer = -1)
        {
            AccesoDatos datosChofer = new AccesoDatos();
            List<Chofer> listaChoferes = new List<Chofer>();

            try
            {
                if (idChofer == -1)
                {
                    datosChofer.SetearConsulta("SELECT C.IDCHOFER, C.IDPERSONA, Z.NOMBREZONA, Z.IDZONA, C.IDVEHICULO FROM CHOFER AS C INNER JOIN ZONAS AS Z ON C.IDZONA = Z.IDZONA");
                }
                else
                {
                    datosChofer.SetearConsulta("SELECT C.IDCHOFER, C.IDPERSONA, Z.NOMBREZONA, Z.IDZONA, C.IDVEHICULO FROM CHOFER AS C INNER JOIN ZONAS AS Z ON C.IDZONA = Z.IDZONA WHERE C.IDCHOFER = @IDCHOFER");
                    datosChofer.SetearParametro("@IDCHOFER", idChofer);
                }

                datosChofer.EjecutarConsulta();

                while (datosChofer.Lector.Read())
                {
                    Chofer choferAux = new Chofer();
                    Persona personaAux = new Persona();
                    ChoferNegocio cnAux = new ChoferNegocio();
                    PersonaNegocio perAux = new PersonaNegocio();


                    if ((int)datosChofer.Lector["IDPERSONA"] == -1) //si no encuentra al id persona devuelve -1
                    {
                        return listaChoferes; //retorna la lista al no encontrar una persona
                    }

                    personaAux = perAux.ObtenerPersona((int)datosChofer.Lector["IDPERSONA"]); //asigna la persona a personaAux

                    //Asigna al choferAux los datos de la persona
                    choferAux.Nombres = personaAux.Nombres;
                    choferAux.Apellidos = personaAux.Apellidos;
                    choferAux.DNI = personaAux.DNI;
                    choferAux.FechaNacimiento = personaAux.FechaNacimiento;
                    choferAux.Direccion = personaAux.Direccion;
                    choferAux.Nacionalidad = personaAux.Nacionalidad;
                    choferAux.IDPersona = personaAux.IDPersona;

                    //asigna el id Chofer
                    choferAux.IDChofer = (int)datosChofer.Lector["IDCHOFER"];

                    //lee la zona y la asigna
                    choferAux.ZonaAsignada = cnAux.ObtenerZonas(datosChofer.Lector["IDZONA"] is DBNull ? 0 : (int)datosChofer.Lector["IDZONA"])[0];

                    //lee el id vehiculo asignado
                    if (datosChofer.Lector["IDVEHICULO"] != null)
                    {
                        int IDVehiculo = (int)datosChofer.Lector["IDVEHICULO"];//guarda el id del vehiculo
                        Vehiculo vehiculoAux = new Vehiculo();
                        VehiculoNegocio vnAux = new VehiculoNegocio();
                        List<Vehiculo> listaVehiculos = new List<Vehiculo>();

                        listaVehiculos = vnAux.ObtenerDatos(); //carga la lista de todos los vehiculo de la BBDD

                        vehiculoAux = listaVehiculos.Find(x => x.IDVehiculo == IDVehiculo); //busca por el ID y asigna el vehiculo a vehiculoAux 
                        
                        choferAux.AutoAsignado = vehiculoAux; //setea el auto del chofer
                    }

                    listaChoferes.Add(choferAux);
                }

                return listaChoferes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datosChofer.CerrarConexion();
            }
        }

        public void BajaChofer(int idChofer)
        {
            AccesoDatos datos = new AccesoDatos();
            PersonaNegocio perAux = new PersonaNegocio();
            DomicilioNegocio domiAux = new DomicilioNegocio();
            Chofer choAux = new Chofer();

            choAux = ObtenerDatos(idChofer)[0];

            try
            {
                perAux.BajaPersona(choAux.IDPersona);

                domiAux.BajaDomicilio(choAux.Direccion.IDDomicilio);

                datos.SetearConsulta("DELETE FROM CHOFER WHERE IDCHOFER = @IDCHOFER");
                datos.SetearParametro("@IDCHOFER", idChofer);

                datos.EjecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public void AltaModificacionChofer(Chofer choferAux, bool esAlta)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                DomicilioNegocio domiAux = new DomicilioNegocio();
                PersonaNegocio perAux = new PersonaNegocio();
                Persona aux = new Persona();

                if (!esAlta)
                {
                    domiAux.AltaModificacionDomicilio(choferAux.Direccion, false);

                    aux.IDPersona = choferAux.IDPersona;
                    aux.Nombres = choferAux.Nombres;
                    aux.Apellidos = choferAux.Apellidos;
                    aux.DNI = choferAux.DNI;
                    aux.FechaNacimiento = choferAux.FechaNacimiento;
                    aux.Direccion = choferAux.Direccion;
                    aux.Nacionalidad = choferAux.Nacionalidad;

                    perAux.AltaModificacionPersona(aux, false);

                    datos.SetearConsulta("UPDATE CHOFER SET IDZONA = @IDZONA, IDVEHICULO = @IDVEHICULO WHERE IDCHOFER = @IDCHOFER");
                    datos.SetearParametro("@IDZONA", choferAux.ZonaAsignada.IDZona);
                    datos.SetearParametro("@IDVEHICULO", choferAux.AutoAsignado.IDVehiculo);
                    datos.SetearParametro("@IDCHOFER", choferAux.IDChofer);
                }
                else
                {
                    domiAux.AltaModificacionDomicilio(choferAux.Direccion, true);

                    long idDomicilio = domiAux.ultimoIdDomicilio();//obtiene el ultimo id de domicilio

                    choferAux.Direccion.IDDomicilio = idDomicilio;

                    aux.Nombres = choferAux.Nombres;
                    aux.Apellidos = choferAux.Apellidos;
                    aux.DNI = choferAux.DNI;
                    aux.FechaNacimiento = choferAux.FechaNacimiento;
                    aux.Direccion = choferAux.Direccion;
                    aux.Nacionalidad = choferAux.Nacionalidad;

                    perAux.AltaModificacionPersona(aux, true);

                    int idPersona = perAux.ultimoIdPersona();//obtiene el ultimo id de persona

                    datos.SetearConsulta("INSERT INTO CHOFER (IDPERSONA, IDZONA, IDVEHICULO) VALUES (@IDPERSONA, @IDZONA, @IDVEHICULO)");
                    datos.SetearParametro("@IDPERSONA", idPersona);//setea el  idPersona recien insertado
                    datos.SetearParametro("@IDZONA", choferAux.ZonaAsignada.IDZona);
                    datos.SetearParametro("@IDVEHICULO", choferAux.AutoAsignado.IDVehiculo);
                }

                datos.EjecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }
    }
}
